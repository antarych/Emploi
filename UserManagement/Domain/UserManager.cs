using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using UserManagement.Application;
using UserManagement.Infrastructure;
using Journalist;
using ProjectManagement.Domain;
using ProjectManagement.Infrastructure;
using TagManagement.Application;

namespace UserManagement.Domain
{
    public class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfirmationRepository _confirmationRepository;
        private readonly ITagManager _tagManager;
        private readonly IProjectRepository _projectRepository;

        public UserManager(IUserRepository userRepository, 
            IConfirmationRepository confirmationRepository,
            ITagManager tagManager,
            IProjectRepository projectRepository
            )
        {
            _confirmationRepository = confirmationRepository;
            _userRepository = userRepository;
            _tagManager = tagManager;
            _projectRepository = projectRepository;
        }

        public int CreateUser(CreateAccountRequest request)
        {
            Require.NotNull(request, nameof(request));
            if (_userRepository.GetAllAccounts(user => user.Email.Equals(request.Email)).SingleOrDefault() != null)
            {
                throw new AccountAlreadyExistsException("Account with such mail already exists");
            }
            var newAccount = new Account(
                request.Email,
                new Password(request.Password), 
                DateTime.Now,
                AccountRoles.User, 
                ConfirmationStatus.NotConfirmed);
            var userId = _userRepository.CreateAccount(newAccount);
            return userId;
        }

        public void UpdateUser(Account request)
        {
            Require.NotNull(request, nameof(request));

            var account = _userRepository.GetAccount(request.UserId);

            if (account == null)
            {
                throw new AccountNotFoundException();
            }
            var confirmationRequest = _confirmationRepository.GetAllConfirmationRequests(
                r => r.UserId == request.UserId && r.ConfirmationType == ConfirmationType.ProfileFilling).SingleOrDefault();
            if (confirmationRequest != null)
            {
                _confirmationRepository.DeleteConfirmationToken(confirmationRequest);
            }
            _userRepository.UpdateAccount(account);
        }

        public void ConfirmUser(int userId)
        {
            var account = GetUser(userId);
            if (account.ConfirmationStatus == ConfirmationStatus.NotConfirmed)
            {
                account.ConfirmationStatus = ConfirmationStatus.MailConfirmed;
                _userRepository.UpdateAccount(account);
                var confirmationRequest = _confirmationRepository.GetAllConfirmationRequests(
                        r => r.UserId == userId && r.ConfirmationType == ConfirmationType.MailConfirmation)
                    .SingleOrDefault();
                _confirmationRepository.DeleteConfirmationToken(confirmationRequest);
            }
        }

        public Account GetUser(int userId)
        {
            Require.Positive(userId, nameof(userId));

            var account = _userRepository.GetAccount(userId);
            if (account == null)
            {
                throw new AccountNotFoundException("Account not found");
            }
            return account;
        }


        public IEnumerable<Account> GetAccounts(Func<Account, bool> predicate = null)
        {
            return _userRepository.GetAllAccounts(predicate);
        }

        public int GetUserByToken(string confirmationToken)
        {
            var confirmationRequest = _confirmationRepository.GetConfirmationRequest(confirmationToken);
            if (confirmationRequest == null)
            {
                throw new IncorrectTokenException("Invalid token");
            }
            return GetUser(confirmationRequest.UserId).UserId;
        }

        public void AddTagsToUser(IList<string> tags, int userId)
        {
            Require.Positive(userId, nameof(userId));
            Require.NotNull(tags, nameof(tags));
            var account = _userRepository.GetAccount(userId);
            ISet<Tag> userTags = new HashSet<Tag>();
            foreach (var tag in tags)
            {
                int tagId;
                if (_tagManager.TryFindTag(tag, out tagId))
                {
                    var tagToAdd = _tagManager.GetTag(t => t.TagId == tagId).SingleOrDefault();
                    userTags.Add(tagToAdd);
                }
                else
                {
                    var newTag = _tagManager.CreateTag(tag);
                    userTags.Add(newTag);
                }
            }
            account.Profile.Tags = userTags;
            _userRepository.UpdateAccount(account);
        }

        public IList<Account> GetMembers(Project project)
        {
            Require.NotNull(project, nameof(project));
            var members = new List<Account>();
            members.AddRange(GetAccounts(user => user.Profile.Portfolio.ToList().Intersect(project.Vacancies).Any()));
            members.Add(_userRepository.GetAccount(project.Leader));
            return members.ToList();
        }

        public IList<Account> GetUsersByTags(IList<string> tags, string course, string institute, string str)
        {
            var tagList = new List<Tag>();
            var userList = new List<Account>();
            if (tags.Count != 0)
            {
                foreach (var tag in tags)
                {
                    int tagId;
                    Tag singleTag = null;
                    if (_tagManager.TryFindTag(tag.ToLower(), out tagId))
                    {
                        singleTag = _tagManager.GetTag(t => t.TagId == tagId).Single();
                    }
                    tagList.Add(singleTag);
                }
                var dictionary = new Dictionary<Account, int>();
                var mostSuitable =
                    _userRepository.GetAllAccounts(account => account.Profile.Tags.ToList().Intersect(tagList).Any());
                foreach (var account in mostSuitable)
                {
                    dictionary.Add(account, account.Profile.Tags.ToList().Intersect(tagList).Count());
                }
                foreach (var pair in dictionary.OrderBy(pair => pair.Value))
                {
                    userList.Add(pair.Key);
                }
                userList.Reverse();
            }

            if (!string.IsNullOrEmpty(course))
            {
                var c = int.Parse(course);
                if (c < 1)
                {
                    throw new ArgumentOutOfRangeException("Invalid course");
                }
                if (userList.Count == 0)
                {
                    userList = GetAccounts(usr => usr.ConfirmationStatus == ConfirmationStatus.MailConfirmed
                    && usr.Profile.Course == c).ToList();
                }
                else
                    userList = userList.FindAll(usr => usr.ConfirmationStatus == ConfirmationStatus.MailConfirmed
                    && usr.Profile.Course == c);
            }
            if (!string.IsNullOrEmpty(institute))
            {

                Institutes inst;
                try
                {
                    inst = (Institutes) Enum.Parse(typeof(Institutes), institute);
                    if (userList.Count == 0)
                    {
                        userList = GetAccounts(usr => usr.ConfirmationStatus == ConfirmationStatus.MailConfirmed
                    && !string.IsNullOrEmpty(usr.Profile.Institute)
                    && usr.Profile.Institute == inst.ToString()).ToList();
                    }
                    else
                    userList = userList.FindAll(usr => usr.ConfirmationStatus == ConfirmationStatus.MailConfirmed
                    && !string.IsNullOrEmpty(usr.Profile.Institute)
                    && usr.Profile.Institute == inst.ToString());
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException("Invalid institute");
                }
            }
            if (!string.IsNullOrEmpty(str))
            {
                if (userList.Count == 0)
                {
                    userList = GetAccounts(usr => usr.ConfirmationStatus == ConfirmationStatus.MailConfirmed
                    && !string.IsNullOrEmpty(usr.Profile.Name) && !string.IsNullOrEmpty(usr.Profile.Surname)
                    && (usr.Profile.Name.StartsWith(str) || usr.Profile.Surname.StartsWith(str))).ToList();
                }
                else
                userList =
                    userList.FindAll(usr => usr.ConfirmationStatus == ConfirmationStatus.MailConfirmed
                    && !string.IsNullOrEmpty(usr.Profile.Name) && !string.IsNullOrEmpty(usr.Profile.Surname) 
                    && (usr.Profile.Name.StartsWith(str) || usr.Profile.Surname.StartsWith(str)));
            }
            return userList;
        }

        public int GetAssignee(Vacancy vacancy, IList<Account> members)
        {
            var account = members.FirstOrDefault(user => user.Profile.Portfolio.Contains(vacancy));
            if (account == null)
            {
                return 0;
            }
            return account.UserId;
        }

        public IList<Project> GetAllUserProjects(Account userAccount)
        {
            var allProjects = new HashSet<Project>();
            foreach (var vac in userAccount.Profile.Portfolio)
            {
                var project = _projectRepository.GetAllProjects(prj => prj.Vacancies.Contains(vac)).SingleOrDefault();
                if (project != null)
                    allProjects.Add(project);
            }
            var leadProjects = _projectRepository.GetAllProjects(prj => prj.Leader == userAccount.UserId);
            foreach (var prj in leadProjects)
            {
                allProjects.Add(prj);
            }
            return allProjects.ToList();
        }

        public IEnumerable<string> GetOfferedTags(string subTag)
        {
            var offeredTagsStr = new List<string>();
            if (string.IsNullOrEmpty(subTag))
            {
                var popularTags = _tagManager.GetPopularTags();
                foreach (var tag in popularTags)
                {
                    offeredTagsStr.Add(tag);
                }
            }
            else
            {
                var offeredTags = _tagManager.GetOfferedTags(subTag);
                foreach (var tag in offeredTags)
                {
                    offeredTagsStr.Add(tag.TagName);
                }
            }
            return offeredTagsStr;
        }
     }
}
