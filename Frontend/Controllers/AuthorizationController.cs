using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Common;
using Frontend.Models;
using Journalist;
using UserManagement.Application;
using UserManagement.Domain;

namespace Frontend.Controllers
{

    public class AuthorizationController : ApiController
    {
        private readonly IAuthorizer _authorizer;
        private readonly IUserManager _userManager;

        public AuthorizationController(IAuthorizer authorizer, IUserManager userManager)
        {
            _authorizer = authorizer;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Authorize([FromBody] LoginInformation loginInformation)
        {
            AuthorizationTokenInfo token;
            try
            {
                token = _authorizer.Authorize(loginInformation.Mail,
                    new Password(loginInformation.Password));
            }
            catch (AccountNotFoundException ex)
            {
                return Content(HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (IncorrectPasswordException ex)
            {
                return Content(HttpStatusCode.Unauthorized, ex.Message);
            }
            if (!_authorizer.CheckProfileCompleteness(loginInformation.Mail))
            {
                var userId = _authorizer.GetUserByMail(loginInformation.Mail).UserId;
                var tokenReg = _authorizer.SaveProfileCompletenessConfirmationRequest(userId);
                return Ok(new ProfileIsNotCompletedResponse(tokenReg));
            }
            var account = _authorizer.GetUserByMail(loginInformation.Mail);
            var projects = _userManager.GetAllUserProjects(account);
            var portfolio = new List<ProjectPresentation>();
            foreach (var prj in projects)
            {
                portfolio.Add(new ProjectPresentation(prj, _userManager.GetMembers(prj)));
            }
            return Ok(new CurrentUserPresentation(account, token, portfolio));                      
        }

        [HttpPost]
        [Route("logOut")]
        public IHttpActionResult LogOut()
        {
            if (Request.Headers.Authorization == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            var token = Request.Headers.Authorization.ToString();
            var tokenString = token.Substring("Basic ".Length).Trim();


            if (_authorizer.GetTokenInfo(tokenString) == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            _authorizer.LogOut(tokenString);
            return Ok();
        }
    }
}
