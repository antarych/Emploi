using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using Frontend.Models;
using Journalist;
using OrganizationManagement.Application;
using OrganizationManagement.Domain;
using UserManagement.Application;

namespace Frontend.Controllers
{
    public class OrganizationController : ApiController
    {
        private readonly IOrganizationManager _orgManager;
        private readonly IUserManager _userManager;

        public OrganizationController(IOrganizationManager orgManager, IUserManager userManager)
        {
            _orgManager = orgManager;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("organizations/creation")]
        public IHttpActionResult CreateNewOrganization([FromBody]OrgCreationModel orgCreationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            CreateOrgRequest orgRequest;
            {
                orgRequest = new CreateOrgRequest(
                    orgCreationModel.Leader,
                    orgCreationModel.OrganizationName,
                    orgCreationModel.OrganizationDescription,
                    orgCreationModel.OrganizationTags);
            }
            var createdOrgId = _orgManager.CreateOrganization(orgRequest);

            return Ok(createdOrgId);
        }

        [HttpGet]
        [Route("organization/{orgId}")]
        public IHttpActionResult GetOrganization(int orgId)
        {
            Require.Positive(orgId, nameof(orgId));
            return Ok(_orgManager.GetOrganization(orgId));
        }

        [HttpPut]
        [Route("organization/{orgId}/update")]
        public IHttpActionResult UpdateOrganization(int orgId, [FromBody]OrgCreationModel orgCreationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Require.Positive(orgId, nameof(orgId));
            var organizationToUpdate = _orgManager.GetOrganization(orgId);
            organizationToUpdate.Leader = orgCreationModel.Leader;
            organizationToUpdate.OrganizationName = orgCreationModel.OrganizationName;
            organizationToUpdate.OrganizationDescription = orgCreationModel.OrganizationDescription;

            return Ok(_orgManager.UpdateOrg(organizationToUpdate));
        }

        [HttpPost]
        [Route("organization/{orgId}/delete")]
        public IHttpActionResult DeleteOrganization(int orgId)
        {
            Require.Positive(orgId, nameof(orgId));

            _orgManager.RemoveOrganization(orgId);

            return Ok();
        }

        [HttpPut]
        [Route("organization/{orgId}/tags")]
        public IHttpActionResult AddTagsToOrganization([FromBody] TagListModel tagList, [FromUri] int orgId)
        {
            Require.Positive(orgId, nameof(orgId));

            try
            {
                _orgManager.AddTagsToOrganization(tagList.tags, orgId);
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest();
            }
            catch (OrgExceptions.OrganizationNotFoundException ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
            return Ok();
        }

        [HttpPost]
        [Route("organization/{orgId}/add/{userId}")]
        public IHttpActionResult AddUserToOrganization(int orgId, int userId)
        {
            Require.Positive(orgId, nameof(orgId));
            Require.Positive(userId, nameof(userId));
            var user = _userManager.GetUser(userId);
            var org = _orgManager.GetOrganization(orgId);
            var organizations = user.Profile.Organizations.ToList();
            organizations.Add(org);
            user.Profile.Organizations = organizations;
            _userManager.UpdateUser(user);
            return Ok();
        }
    }
}
