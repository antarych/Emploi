using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Web.WebPages;
using FileManagement;
using UserManagement.Application;
using Frontend.Models;
using Newtonsoft.Json;
using UserManagement.Domain;

namespace Frontend.Controllers
{
    public class UsersController : ApiController
    {
        protected UsersController() { }

        public UsersController(IUserManager userManager, 
            IAuthorizer authorizer, 
            IMailingService mailingService, 
            IFileManager fileManager)
        {
            _userManager = userManager;
            _authorizer = authorizer;
            _mailingService = mailingService;
            _fileManager = fileManager;
        }
        private readonly IMailingService _mailingService;
        private readonly IUserManager _userManager;
        private readonly IAuthorizer _authorizer;
        private readonly IFileManager _fileManager;

        [HttpPost]
        [Route("users")]
        public IHttpActionResult RegisterNewUser([FromBody] UserRegistrationModel userRegistrationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            CreateAccountRequest accountRequest;
            try
            {
                accountRequest = new CreateAccountRequest(
                    userRegistrationModel.Password,
                    new MailAddress(userRegistrationModel.Mail));
            }
            catch (System.ArgumentException)
            {
                return BadRequest("Fields must not be empty");
            }
            int createdUserId;
            try
            {
                createdUserId = _userManager.CreateUser(accountRequest);
            }
            catch (AccountAlreadyExistsException ex)
            {
                return Content(HttpStatusCode.Conflict, ex.Message);
            }
            _mailingService.SetupEmailConfirmation(createdUserId);
            return Ok(createdUserId);
        }

        [HttpGet]
        [Route("users/{userId}")]
        public IHttpActionResult GetUser(int userId)
        {
            Account account;
            try
            {
                account = _userManager.GetUser(userId);
            }
            catch (AccountNotFoundException ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
            if (account.ConfirmationStatus == ConfirmationStatus.MailConfirmed)
            {
                var projects = _userManager.GetAllUserProjects(account);
                var portfolio = new List<ProjectPresentation>();
                foreach (var prj in projects)
                {
                    portfolio.Add(new ProjectPresentation(prj, _userManager.GetMembers(prj)));
                }
                return Ok(new UserPresentation(account.UserId, account.Email.ToString(), account.Profile, portfolio));
            }
            return Content(HttpStatusCode.NotFound, "Account not found");
        }

        [HttpGet]
        [Route("current")]
        public IHttpActionResult GetCurrentUser()
        {
            if (Request.Headers.Authorization == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            var token = Request.Headers.Authorization.ToString();
            var tokenString = token.Substring("Basic ".Length).Trim();
            if (!token.IsEmpty() && token.StartsWith("Basic"))
            {
                if (_authorizer.GetTokenInfo(tokenString) == null)
                {
                    return Content(HttpStatusCode.Unauthorized, "Invalid token");
                }
            }
            var tokenInfo = _authorizer.GetTokenInfo(tokenString);
            if (tokenInfo == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            Account account;
            try
            {
                account = _userManager.GetUser(tokenInfo.UserId);
            }
            catch (AccountNotFoundException ex)
            {
                return Content(HttpStatusCode.Unauthorized, ex.Message);
            }
            var projects = _userManager.GetAllUserProjects(account);
            var portfolio = new List<ProjectPresentation>();
            foreach (var prj in projects)
            {
                portfolio.Add(new ProjectPresentation(prj, _userManager.GetMembers(prj)));
            }
            return Ok(new UserPresentation(account.UserId, account.Email.ToString(), account.Profile, portfolio));
        }

        [HttpPut]
        [Route("users")]
        public IHttpActionResult UpdateUser([FromBody] UserUpdateModel updateProfileRequest)
        {
            if (Request.Headers.Authorization == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            var token = Request.Headers.Authorization.ToString();
            var tokenString = token.Substring("Basic ".Length).Trim();
            if (!token.IsEmpty() && token.StartsWith("Basic"))
            {        
                if (_authorizer.GetTokenInfo(tokenString) == null && _userManager.GetUser(_userManager.GetUserByToken(tokenString)) == null)
                {
                    return Content(HttpStatusCode.Unauthorized, "Invalid token");
                }
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Account accountToChange;
            try
            {
                accountToChange = _userManager.GetUser(_userManager.GetUserByToken(tokenString));
            }
            catch (IncorrectTokenException e)
            {
                accountToChange = _userManager.GetUser(_authorizer.GetTokenInfo(tokenString).UserId);
            }

            if (updateProfileRequest != null)
            {
                if (updateProfileRequest.Avatar != null)
                {
                    try
                    {
                        _fileManager.GetImage(updateProfileRequest.Avatar);
                        accountToChange.Profile.Avatar = updateProfileRequest.Avatar;
                    }
                    catch (FileNotFoundException ex)
                    {
                        return Content(HttpStatusCode.BadRequest, ex.Message);
                    }            
                }
                if (updateProfileRequest.Name != null)
                    accountToChange.Profile.Name = updateProfileRequest.Name;
                if (updateProfileRequest.Surname != null)
                    accountToChange.Profile.Surname = updateProfileRequest.Surname;
                if (updateProfileRequest.Middlename != null)
                    accountToChange.Profile.Middlename = updateProfileRequest.Middlename;
                if (updateProfileRequest.AboutUser != null)
                    accountToChange.Profile.AboutUser = updateProfileRequest.AboutUser;
                if (updateProfileRequest.Contacts != null)
                    accountToChange.Profile.Contacts = updateProfileRequest.Contacts;
                if (updateProfileRequest.Course != 0)
                    accountToChange.Profile.Course = updateProfileRequest.Course;
                if (updateProfileRequest.Direction != null)
                    accountToChange.Profile.Direction = updateProfileRequest.Direction;
                if (updateProfileRequest.Institute != null)
                    accountToChange.Profile.Institute = updateProfileRequest.Institute.ToString();
            }

            try
            {
                _userManager.UpdateUser(accountToChange);
            }
            catch (System.ArgumentException)
            {
                return Content(HttpStatusCode.Unauthorized, "Account not found");
            }
            var projects = _userManager.GetAllUserProjects(accountToChange);
            var portfolio = new List<ProjectPresentation>();
            foreach (var prj in projects)
            {
                portfolio.Add(new ProjectPresentation(prj, _userManager.GetMembers(prj)));
            }
            return Ok(new UserPresentation(accountToChange, portfolio));
        }

        [HttpPut]
        [Route("users/tags")]
        public IHttpActionResult AddTagsToUser([FromBody] TagListModel tagList)
        {
            if (Request.Headers.Authorization == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            var token = Request.Headers.Authorization.ToString();
            var tokenString = token.Substring("Basic ".Length).Trim();
            if (!token.IsEmpty() && token.StartsWith("Basic"))
            {       
                if (_authorizer.GetTokenInfo(tokenString) == null)
                {
                    return Content(HttpStatusCode.Unauthorized, "Invalid token");
                }
            }
            else
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            try
            {
                _userManager.AddTagsToUser(tagList.tags, _authorizer.GetTokenInfo(tokenString).UserId);
            }
            catch (System.ArgumentException)
            {
                return BadRequest();
            }
            var projects = _userManager.GetAllUserProjects(_userManager.GetUser(_authorizer.GetTokenInfo(tokenString).UserId));
            var portfolio = new List<ProjectPresentation>();
            foreach (var prj in projects)
            {
                portfolio.Add(new ProjectPresentation(prj, _userManager.GetMembers(prj)));
            }
            return Ok(new UserPresentation(_userManager.GetUser(_authorizer.GetTokenInfo(tokenString).UserId), portfolio));
        }

        [HttpPost]
        [Route("users/confirm/{token}")]
        public IHttpActionResult ConfirmUser([FromUri] string token)
        {
            try
            {
                var userId = _userManager.GetUserByToken(token);
                _userManager.ConfirmUser(userId);
            }
            catch (IncorrectTokenException ex)
            {
                return Content(HttpStatusCode.Unauthorized, ex.Message);
            }
            return Ok();
        }

        [HttpGet]
        [Route("users/search/{searchByPageString}")]
        public IHttpActionResult FindUsersByTags([FromUri] string searchByPageString)
        {
            var allUri = HttpContext.Current.Request.RawUrl;
            var stringToParse = allUri.Split('?');
            if (stringToParse[1] == null)
            {
                return Content(HttpStatusCode.BadRequest, "Search request is empty");
            }

            var page = HttpUtility.ParseQueryString(stringToParse[1]).Get("page");
            if (page == null)
            {
                return Content(HttpStatusCode.BadRequest, "Page is null");
            }
            var str = HttpUtility.ParseQueryString(stringToParse[1]).Get("str");
            var tags = HttpUtility.ParseQueryString(stringToParse[1]).Get("tags");
            IList<string> deserializedTags = new List<string>();
            if (tags != null)
            {
                deserializedTags = JsonConvert.DeserializeObject<string[]>(tags);
            }
            var course = HttpUtility.ParseQueryString(stringToParse[1]).Get("course");
            var institute = HttpUtility.ParseQueryString(stringToParse[1]).Get("institute");
            if (str == null && deserializedTags == null && course == null && institute == null)
            {
                return Ok(new List<Account>());
            }
            var accounts = new List<Account>();

            try
            {
                accounts = _userManager.GetUsersByTags(deserializedTags, course, institute, str).ToList();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }

            var usersPresentation = new List<UserPresentation>();

            foreach (var account in accounts)
            {
                var projects = _userManager.GetAllUserProjects(account);
                var portfolio = new List<ProjectPresentation>();
                foreach (var prj in projects)
                {
                    portfolio.Add(new ProjectPresentation(prj, _userManager.GetMembers(prj)));
                }
                usersPresentation.Add(new UserPresentation(account, portfolio));
            }
            usersPresentation.ToArray();
            var pagePresentation = new List<UserPresentation>();
            {
                var countOfNotesOnPage = 10;
                var i = countOfNotesOnPage * (int.Parse(page) - 1);
                if (int.Parse(page) == 1) i = 0;
                var countOnPage = 0;
                while (i < usersPresentation.Count && countOnPage < countOfNotesOnPage)
                {
                    pagePresentation.Add(usersPresentation[i]);
                    i++;
                    countOnPage++;
                }
            }
            return Ok(pagePresentation);
        }

        [HttpGet]
        [Route("tags/{subTag}")]
        public IHttpActionResult GetTags([FromUri] string subTag)
        {
            var tags = new TagListModel();
            tags.tags = _userManager.GetOfferedTags(subTag).ToList();
            return Ok(tags);
        }

        [HttpGet]
        [Route("tags")]
        public IHttpActionResult GetPopTags()
        {
            string subTag = "";
            var tags = new TagListModel();
            tags.tags = _userManager.GetOfferedTags(subTag).ToList();
            return Ok(tags);
        }
    }
}