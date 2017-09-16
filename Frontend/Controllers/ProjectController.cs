using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Frontend.Models;
using ProjectManagement.Application;
using ProjectManagement.Domain;
using UserManagement.Application;
using UserManagement.Domain;

namespace Frontend.Controllers
{
    public class ProjectController : ApiController
    {
        private readonly IProjectManager _projectManager;
        private readonly IUserManager _userManager;
        private readonly IAuthorizer _authorizer;

        public ProjectController(IProjectManager projectManager, IUserManager userManager, IAuthorizer authorizer)
        {
            _projectManager = projectManager;
            _userManager = userManager;
            _authorizer = authorizer;
        }

        [HttpPost]
        [Route("projects")]
        public IHttpActionResult CreateNewProject([FromBody]ProjectCreationModel prjCreationModel)
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

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            CreateProjectRequest projectRequest;
            try
            {
                projectRequest = new CreateProjectRequest(
                    _authorizer.GetTokenInfo(tokenString).UserId,
                    prjCreationModel.avatar,
                    prjCreationModel.name,
                    prjCreationModel.description,
                    prjCreationModel.isFromOrganization,
                    prjCreationModel.organizationId);                
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest();
            }
            var createdProjectId = _projectManager.CreateProject(projectRequest);
            if (prjCreationModel.tags != null)
                _projectManager.AddTagsToProject(prjCreationModel.tags, createdProjectId);
            return Ok(createdProjectId);
        }

        [HttpPut]
        [Route("projects/{projectId}")]
        public IHttpActionResult UpdateProject([FromUri] int projectId, [FromBody] ProjectUpdateModel prjUpdateModel)
        {
            if (Request.Headers.Authorization == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            var token = Request.Headers.Authorization.ToString();
            var tokenString = token.Substring("Basic ".Length).Trim();

            var project = _projectManager.GetProject(projectId);

            if (_authorizer.GetTokenInfo(tokenString) == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            if (project.Leader != _authorizer.GetTokenInfo(tokenString).UserId)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (prjUpdateModel.PrjImage != null) project.ProjectImage = prjUpdateModel.PrjImage;
            if (prjUpdateModel.ProjectName != null) project.ProjectName = prjUpdateModel.ProjectName;
            if (prjUpdateModel.ProjectDescription != null) project.ProjectDescription = prjUpdateModel.ProjectDescription;
            if (prjUpdateModel.Tags != null)
                _projectManager.AddTagsToProject(prjUpdateModel.Tags, project.ProjectId);
            _projectManager.UpdateProject(project);
            return Ok(new ProjectPresentation(project, _userManager.GetMembers(project)));
        }

        [HttpGet]
        [Route("projects/{projectId}")]
        public IHttpActionResult GetProject([FromUri] int projectId)
        {
            Project project;
            try
            {
                project = _projectManager.GetProject(projectId);
            }
            catch (ArgumentOutOfRangeException)
            {
                return Content(HttpStatusCode.NotFound, "Project not found");
            }
            if (project == null)
            {
                return Content(HttpStatusCode.NotFound, "Project not found");
            }
            IList<Account> members;
            try
            {
                members = _userManager.GetMembers(project);
            }
            catch (ArgumentOutOfRangeException)
            {
                return Content(HttpStatusCode.NotFound, "Project not found");
            }
            return Ok(new ProjectPresentation(project, members));
        }

        [HttpGet]
        [Route("projects/{projectId}/vacancies/{vacancyId}")]
        public IHttpActionResult GetVacancy([FromUri] int vacancyId, int projectId)
        {
            Project project;
            Vacancy vacancy;
            try
            {
                project = _projectManager.GetProject(projectId);
            }
            catch (ArgumentOutOfRangeException)
            {
                return Content(HttpStatusCode.NotFound, "Project not found");
            }
            if (project == null)
            {
                return Content(HttpStatusCode.NotFound, "Project not found");
            }
            try
            {
                vacancy = _projectManager.GetVacancy(vac => vac.VacancyId == vacancyId).SingleOrDefault();
            }
            catch (ArgumentOutOfRangeException)
            {
                return Content(HttpStatusCode.NotFound, "Vacancy not found");
            }
            if (vacancy == null)
            {
                return Content(HttpStatusCode.NotFound, "Vacancy not found");
            }
            var proj = _projectManager.GetProjects(prj => prj.Vacancies.Contains(vacancy)).SingleOrDefault();
            if (proj == null)
            {
                return Content(HttpStatusCode.BadRequest, "Vacancy doesn't belong to this project");
            }
            if (proj.ProjectId != projectId)
            {
                return Content(HttpStatusCode.BadRequest, "Vacancy doesn't belong to this project");
            }
            var members = _userManager.GetMembers(project);
            return Ok(new VacancyPresentation(vacancy, members));
        }

        [HttpPost]
        [Route("projects/{projectId}/delete")]
        public IHttpActionResult RemoveProject([FromUri] int projectId)
        {
            if (Request.Headers.Authorization == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            var token = Request.Headers.Authorization.ToString();
            var tokenString = token.Substring("Basic ".Length).Trim();

            var project = _projectManager.GetProject(projectId);

            if (_authorizer.GetTokenInfo(tokenString) == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            if (project.Leader != _authorizer.GetTokenInfo(tokenString).UserId)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            try
            {
                var members = _userManager.GetMembers(project);
                foreach (var vac in project.Vacancies)
                {
                    var userId = _userManager.GetAssignee(vac, members);
                    _projectManager.RemoveVacancy(project, vac, userId);
                }
                _projectManager.RemoveProject(projectId);
            }
            catch (ArgumentOutOfRangeException)
            {
                return Content(HttpStatusCode.NotFound, "Project not found");
            }
            return Ok();
        }

        [HttpPut]
        [Route("projects/{projectId}/vacancies/{vacancyId}/assign")]
        public IHttpActionResult AddMemberToProject([FromUri] int vacancyId, int projectId, [FromBody] AssignUserModel userId)
        {
            if (Request.Headers.Authorization == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            var token = Request.Headers.Authorization.ToString();
            var tokenString = token.Substring("Basic ".Length).Trim();

            var project = _projectManager.GetProject(projectId);

            if (_authorizer.GetTokenInfo(tokenString) == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            if (project.Leader != _authorizer.GetTokenInfo(tokenString).UserId)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            if (_projectManager.GetProjects(prj => prj.Vacancies.Any(vaca => vaca.VacancyId == vacancyId)).SingleOrDefault() == null)
            {
                return Content(HttpStatusCode.BadRequest, "Vacancy doesn't belong to this project");
            }

            if (_projectManager.GetProjects(prj => prj.Vacancies.Any(vaca => vaca.VacancyId == vacancyId)).Single().ProjectId != projectId)
            {
                return Content(HttpStatusCode.BadRequest, "Vacancy doesn't belong to this project");
            }
            try
            {
                _projectManager.AddMemberToProject(vacancyId, userId.memberId);
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest();
            }
            catch (AccountNotFoundException ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
            catch (VacancyNotFoundException ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
            return Ok();
        }

        [HttpPost]
        [Route("projects/{projectId}/vacancies/{vacancyId}/unassign")]
        public IHttpActionResult RemoveMemberFromProject([FromUri] int vacancyId, int projectId)
        {
            if (Request.Headers.Authorization == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            var token = Request.Headers.Authorization.ToString();
            var tokenString = token.Substring("Basic ".Length).Trim();

            var project = _projectManager.GetProject(projectId);

            if (_authorizer.GetTokenInfo(tokenString) == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            if (project.Leader != _authorizer.GetTokenInfo(tokenString).UserId)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            if (
                _projectManager.GetProjects(prj => prj.Vacancies.Any(vaca => vaca.VacancyId == vacancyId))
                    .SingleOrDefault() == null)
            {
                return Content(HttpStatusCode.BadRequest, "Vacancy doesn't belong to this project");
            }

            if (
                _projectManager.GetProjects(prj => prj.Vacancies.Any(vaca => vaca.VacancyId == vacancyId))
                    .Single()
                    .ProjectId != projectId)
            {
                return Content(HttpStatusCode.BadRequest, "Vacancy doesn't belong to this project");
            }
            IList<Account> members;
            try
            {
                members = _userManager.GetMembers(project);
            }
            catch (ArgumentOutOfRangeException)
            {
                return Content(HttpStatusCode.NotFound, "Project not found");
            }
            try
            {
                var vacancy = _projectManager.GetVacancy(vac => vac.VacancyId == vacancyId).FirstOrDefault();
                if (vacancy == null)
                {
                    return Content(HttpStatusCode.NotFound, "Vacancy not found");
                }
                var userId = _userManager.GetAssignee(vacancy, members);
                if (userId != 0)
                {
                    _projectManager.RemoveMemberFromProject(vacancyId, userId);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest();
            }
            catch (AccountNotFoundException ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
            catch (VacancyNotFoundException ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
            return Ok();
        }

        [HttpPost]
        [Route("projects/{projectId}/vacancies")]
        public IHttpActionResult CreateNewVacancy([FromUri] int projectId, [FromBody] VacancyCreationModel vacancyModel)
        {
            if (Request.Headers.Authorization == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            var token = Request.Headers.Authorization.ToString();
            var tokenString = token.Substring("Basic ".Length).Trim();

            var project = _projectManager.GetProject(projectId);

            if (_authorizer.GetTokenInfo(tokenString) == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            if (project.Leader != _authorizer.GetTokenInfo(tokenString).UserId)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            var vacancy = new Vacancy(vacancyModel.profession);
            vacancy.Description = vacancyModel.description;
            int vacancyId;
            try
            {
                vacancyId = _projectManager.CreateVacancy(projectId, vacancy);
            }
            catch (ProjectNotFoundException ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }

            if (vacancyModel.tags != null)
            {
                _projectManager.AddTagsToVacancy(vacancyModel.tags, vacancyId);
            }

            return Ok(vacancyId);
        }

        [HttpPut]
        [Route("projects/{projectId}/vacancies/{vacancyId}")]
        public IHttpActionResult UpdateVacancy([FromUri] int projectId, int vacancyId, [FromBody] VacancyUpdateModel vacancyModel)
        {
            if (Request.Headers.Authorization == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            var token = Request.Headers.Authorization.ToString();
            var tokenString = token.Substring("Basic ".Length).Trim();

            var project = _projectManager.GetProject(projectId);

            if (_authorizer.GetTokenInfo(tokenString) == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            if (project.Leader != _authorizer.GetTokenInfo(tokenString).UserId)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            if (
                _projectManager.GetProjects(prj => prj.Vacancies.Any(vaca => vaca.VacancyId == vacancyId))
                    .SingleOrDefault() == null)
            {
                return Content(HttpStatusCode.BadRequest, "Vacancy doesn't belong to this project");
            }

            if (
                _projectManager.GetProjects(prj => prj.Vacancies.Any(vaca => vaca.VacancyId == vacancyId))
                    .Single()
                    .ProjectId != projectId)
            {
                return Content(HttpStatusCode.BadRequest, "Vacancy doesn't belong to this project");
            }

            var vacancy = _projectManager.GetVacancy(vac => vac.VacancyId == vacancyId).SingleOrDefault();

            if (vacancy == null)
            {
                return Content(HttpStatusCode.NotFound, "Vacancy not found");
            }

            if (vacancyModel.profession != null)
            {
                vacancy.Name = vacancyModel.profession;
            }

            if (vacancyModel.description != null)
            {
                vacancy.Description = vacancyModel.description;
            }

            if (vacancyModel.tags != null)
            {
                _projectManager.AddTagsToVacancy(vacancyModel.tags, vacancyId);
            }

            _projectManager.UpdateVacancy(vacancy);

            return Ok(vacancyId);
        }

        [HttpPost]
        [Route("projects/{projectId}/vacancies/{vacancyId}/delete")]
        public IHttpActionResult RemoveVacancy([FromUri] int projectId, int vacancyId)
        {
            if (Request.Headers.Authorization == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            var token = Request.Headers.Authorization.ToString();
            var tokenString = token.Substring("Basic ".Length).Trim();

            var project = _projectManager.GetProject(projectId);

            if (project == null)
            {
                return Content(HttpStatusCode.NotFound, "Project not found");
            }

            if (_authorizer.GetTokenInfo(tokenString) == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            if (project.Leader != _authorizer.GetTokenInfo(tokenString).UserId)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            if (
                _projectManager.GetProjects(prj => prj.Vacancies.Any(vaca => vaca.VacancyId == vacancyId))
                    .SingleOrDefault() == null)
            {
                return Content(HttpStatusCode.BadRequest, "Vacancy doesn't belong to this project");
            }

            if (
                _projectManager.GetProjects(prj => prj.Vacancies.Any(vaca => vaca.VacancyId == vacancyId))
                    .Single()
                    .ProjectId != projectId)
            {
                return Content(HttpStatusCode.BadRequest, "Vacancy doesn't belong to this project");
            }

            var vacancy = _projectManager.GetVacancy(vac => vac.VacancyId == vacancyId).SingleOrDefault();

            if (vacancy == null)
            {
                return Content(HttpStatusCode.NotFound, "Vacancy not found");
            }

            IList<Account> members;
            try
            {
                members = _userManager.GetMembers(project);
            }
            catch (ArgumentOutOfRangeException)
            {
                return Content(HttpStatusCode.NotFound, "Project not found");
            }

            var userId = _userManager.GetAssignee(vacancy, members);

            _projectManager.RemoveVacancy(project, vacancy, userId);
            return Ok();
        }


        [HttpGet]
        [Route("projects/{projectId}/vacancies/{vacancyId}/token")]
        public IHttpActionResult GenerateVacancyToken([FromUri] int projectId, int vacancyId)
        {
            if (Request.Headers.Authorization == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            var token = Request.Headers.Authorization.ToString();
            var tokenString = token.Substring("Basic ".Length).Trim();

            var project = _projectManager.GetProject(projectId);

            if (project == null)
            {
                return Content(HttpStatusCode.NotFound, "Project not found");
            }

            if (_authorizer.GetTokenInfo(tokenString) == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            if (project.Leader != _authorizer.GetTokenInfo(tokenString).UserId)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            if (
                _projectManager.GetProjects(prj => prj.Vacancies.Any(vaca => vaca.VacancyId == vacancyId))
                    .SingleOrDefault() == null)
            {
                return Content(HttpStatusCode.BadRequest, "Vacancy doesn't belong to this project");
            }

            if (
                _projectManager.GetProjects(prj => prj.Vacancies.Any(vaca => vaca.VacancyId == vacancyId))
                    .Single()
                    .ProjectId != projectId)
            {
                return Content(HttpStatusCode.BadRequest, "Vacancy doesn't belong to this project");
            }

            var vacancy = _projectManager.GetVacancy(vac => vac.VacancyId == vacancyId).SingleOrDefault();

            if (vacancy == null)
            {
                return Content(HttpStatusCode.NotFound, "Vacancy not found");
            }
            var vacToken = _projectManager.GenerateVacancyToken(vacancyId);

            return Ok(new VacancyTokenPresentation(vacToken));
        }

        [HttpPost]
        [Route("vacancies/applyByToken")]
        public IHttpActionResult ApplyByToken([FromBody] VacancyTokenRequest vacToken)
        {
            if (Request.Headers.Authorization == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }
            var token = Request.Headers.Authorization.ToString();
            var tokenString = token.Substring("Basic ".Length).Trim();

            var tokenInfo = _authorizer.GetTokenInfo(tokenString);

            if (tokenInfo == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Invalid token");
            }

            var vacancyToken = _projectManager.GetVacancyToken(vacToken.vacancyToken);
            if (vacancyToken == null)
            {
                return Content(HttpStatusCode.NotFound, "Invalid vacancy token");
            }
            var vacancy = _projectManager.GetVacancy(vac => vac.VacancyId == vacancyToken.VacancyId).SingleOrDefault();
            if (vacancy == null)
            {
                return Content(HttpStatusCode.NotFound, "Vacancy not found");
            }
            _projectManager.AddMemberToProject(vacancy.VacancyId, tokenInfo.UserId);
            _projectManager.RemoveVacancyToken(vacancyToken);
            return Ok();
        }

        [HttpPost]
        [Route("projects/search")]
        public IHttpActionResult FindPrjByTags([FromBody] TagListModel tagList)
        {
            var projects = _projectManager.GetProjectsByTags(tagList.tags);
            var projectPresentation = new List<ProjectPresentation>();
            foreach (var project in projects)
            {
                IList<Account> members;
                members = _userManager.GetMembers(project);
                projectPresentation.Add(new ProjectPresentation(project, members));
            }
            return Ok(projectPresentation);
        }

        [HttpPost]
        [Route("vacancies/search")]
        public IHttpActionResult FindVacancyByTags([FromBody] TagListModel tagList)
        {
            var vacancies = _projectManager.GetVacanciesByTags(tagList.tags);
            var vacancyPresentation = new List<VacancyPresentation>();
            foreach (var vacancy in vacancies)
            {
                IList<Account> members;
                var project = _projectManager.GetProjects(prj => prj.Vacancies.Contains(vacancy)).Single();
                members = _userManager.GetMembers(project);
                vacancyPresentation.Add(new VacancyPresentation(vacancy, members));
            }
            return Ok(vacancyPresentation);
        }
    }
}
