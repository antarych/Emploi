using System;
using System.Collections.Generic;
using ProjectManagement.Domain;
using UserManagement.Domain;

namespace UserManagement.Application
{
    public interface IUserManager
    {
        Account GetUser(int userId);

        int CreateUser(CreateAccountRequest request);

        void UpdateUser(Account account);

        void ConfirmUser(int userId);

        IEnumerable<Account> GetAccounts(Func<Account, bool> predicate = null);

        int GetUserByToken(string confirmationToken);

        void AddTagsToUser(IList<string> tags, int userId);

        IList<Account> GetMembers(Project project);

        IList<Account> GetUsersByTags(IList<string> tags, string course, string institute, string str);

        int GetAssignee(Vacancy vacancy, IList<Account> members);

        IList<Project> GetAllUserProjects(Account userAccount);

        IEnumerable<string> GetOfferedTags(string subTag);
    }
}
