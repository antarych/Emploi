using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Journalist;
using ProjectManagement.Application;
using ProjectManagement.Infrastructure;
using TagManagement.Application;

namespace ProjectManagement.Domain
{
    public class ProjectManager : IProjectManager
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITagManager _tagManager;

        public ProjectManager(IProjectRepository projectRepository,
            ITagManager tagManager)
        {
            _projectRepository = projectRepository;
            _tagManager = tagManager;
        }

        public int CreateProject(CreateProjectRequest request)
        {
            Require.NotNull(request, nameof(request));
            var newProject = new Project(
                    request.LeaderId,
                    request.PrjImage,
                    request.ProjectName,
                    request.ProjectDescription,
                    request.IsFromOrganization,
                    request.OrganizationId);
            var leadVacancy = new Vacancy("leader");
            leadVacancy.MemberId = request.LeaderId;
            newProject.Vacancies.ToList().Add(leadVacancy);
            foreach (Vacancy vacancy in newProject.Vacancies)
            {
                var vacancyId = _projectRepository.CreateVacancy(vacancy);
                var userId = vacancy.MemberId;
                if (userId != 0)
                {
                    _projectRepository.AddMemberToProject(userId, vacancyId);
                    vacancy.MemberId = userId;
                }
            }
            return _projectRepository.CreateProject(newProject);
        }

        public void UpdateProject(Project project)
        {
            Require.NotNull(project, nameof(project));
            _projectRepository.UpdateProject(project);
        }

        public Project GetProject(int projectId)
        {
            Require.Positive(projectId, nameof(projectId));

            var project = _projectRepository.GetProject(projectId);

            return project;
        }

        public IEnumerable<Project> GetProjects(Func<Project, bool> predicate = null)
        {
            return _projectRepository.GetAllProjects(predicate);
        }

        public void RemoveProject(int projectId)
        {
            Require.Positive(projectId, nameof(projectId));

            _projectRepository.RemoveProject(projectId);
        }

        public void AddMemberToProject(int vacancyId, int userId)
        {
            Require.Positive(vacancyId, nameof(vacancyId));

            Require.Positive(userId, nameof(userId));

            _projectRepository.AddMemberToProject(userId, vacancyId);
        }

        public void RemoveMemberFromProject(int vacancyId, int userId)
        {
            Require.Positive(vacancyId, nameof(vacancyId));
            Require.Positive(userId, nameof(userId));

            _projectRepository.RemoveMemberFromProject(vacancyId, userId);
        }

        public int CreateVacancy(int projectId, Vacancy vacancy)
        {
            Require.Positive(projectId, nameof(projectId));
            Require.NotNull(vacancy, nameof(vacancy));

            var project = GetProject(projectId);
            if (project == null)
            {
                throw new ProjectNotFoundException("ProjectNotFound");
            }

            var vacId = _projectRepository.CreateVacancy(vacancy);

            var savedVacancy = GetVacancy(vac => vac.VacancyId == vacId).Single();
            var allVacancies = project.Vacancies.ToList();
            allVacancies.Add(savedVacancy);
            project.Vacancies = allVacancies;
            _projectRepository.UpdateProject(project);            
            return vacId;
        }

        public void UpdateVacancy(Vacancy vacancy)
        {
            _projectRepository.UpdateVacancy(vacancy);
        }

        public void RemoveVacancy(Project project, Vacancy vacancy, int userId)
        {            
            _projectRepository.RemoveVacancy(project, vacancy, userId);
        }

        public List<Vacancy> GetVacancy(Func<Vacancy, bool> predicate = null)
        {
            return _projectRepository.GetVacancy(predicate);
        }

        public void AddTagsToProject(IList<string> tags, int projectId)
        {
            Require.Positive(projectId, nameof(projectId));

            Require.NotNull(tags, nameof(tags));

            var project = GetProject(projectId);
            if (project == null)
            {
                throw new ProjectNotFoundException("Project not found");
            }
            ISet<Tag> prjTags = new HashSet<Tag>();
            foreach (var tag in tags)
            {
                int tagId;
                if (_tagManager.TryFindTag(tag, out tagId))
                {
                    var tagToAdd = _tagManager.GetTag(t => t.TagId == tagId).Single();
                    prjTags.Add(tagToAdd);
                }
                else
                {
                    var newTag = _tagManager.CreateTag(tag);
                    prjTags.Add(newTag);
                }
            }
            project.ProjectTags = prjTags;
            _projectRepository.UpdateProject(project);
        }

        public void AddTagsToVacancy(IList<string> tags, int vacancyId)
        {
            Require.Positive(vacancyId, nameof(vacancyId));

            Require.NotNull(tags, nameof(tags));

            var vacancy = _projectRepository.GetVacancy(v => v.VacancyId == vacancyId).SingleOrDefault();
            if (vacancy == null)
            {
                throw new VacancyNotFoundException("Vacancy not found");
            }
            ISet<Tag> vacTags = new HashSet<Tag>();
            foreach (var tag in tags)
            {
                int tagId;

                if (_tagManager.TryFindTag(tag, out tagId))
                {
                    var tagToAdd = _tagManager.GetTag(t => t.TagId == tagId).Single();
                    vacTags.Add(tagToAdd);
                }
                else
                {
                    var newTag = _tagManager.CreateTag(tag);
                    vacTags.Add(newTag);
                }
            }
            vacancy.VacancyTags = vacTags;
            _projectRepository.UpdateVacancy(vacancy);
        }

        public IList<Project> GetProjectsByTags(IList<string> tags)
        {
            Require.NotNull(tags, nameof(tags));
            var tagList = new List<Tag>();
            var projectList = new List<Project>();
            foreach (var tag in tags)
            {
                int tagId;
                Tag singleTag = null;
                if (_tagManager.TryFindTag(tag, out tagId))
                {
                    singleTag = _tagManager.GetTag(t => t.TagId == tagId).Single();
                }
                tagList.Add(singleTag);
            }
            var dictionary = new Dictionary<Project, int>();
            var mostSuitable = _projectRepository.GetAllProjects(prj => prj.ProjectTags.ToList().Intersect(tagList).Any());
            foreach (var prj in mostSuitable)
            {
                dictionary.Add(prj, prj.ProjectTags.ToList().Intersect(tagList).Count());
            }
            foreach (var pair in dictionary.OrderBy(pair => pair.Value))
            {
                projectList.Add(pair.Key);
            }
            projectList.Reverse();
            return projectList;
        }

        public IList<Vacancy> GetVacanciesByTags(IList<string> tags)
        {
            Require.NotNull(tags, nameof(tags));
            var tagList = new List<Tag>();
            var vacancyList = new List<Vacancy>();
            foreach (var tag in tags)
            {
                int tagId;
                Tag singleTag = null;
                if (_tagManager.TryFindTag(tag, out tagId))
                {
                    singleTag = _tagManager.GetTag(t => t.TagId == tagId).Single();
                }
                tagList.Add(singleTag);
            }
            var dictionary = new Dictionary<Vacancy, int>();
            var mostSuitable = _projectRepository.GetVacancy(vac => vac.VacancyTags.ToList().Intersect(tagList).Any());
            foreach (var vac in mostSuitable)
            {
                dictionary.Add(vac, vac.VacancyTags.ToList().Intersect(tagList).Count());
            }
            foreach (var pair in dictionary.OrderBy(pair => pair.Value))
            {
                vacancyList.Add(pair.Key);
            }
            vacancyList.Reverse();
            return vacancyList;
        }

        public VacancyToken GenerateVacancyToken(int vacancyId)
        {
            Require.Positive(vacancyId, nameof(vacancyId));
            var newToken = new VacancyToken();
            newToken.VacancyId = vacancyId;
            newToken.Token = TokenGenerator.GenerateToken();
            _projectRepository.SaveVacancyToken(newToken);
            return newToken;
        }

        public void RemoveVacancyToken(VacancyToken vacToken)
        {
            Require.NotNull(vacToken, nameof(vacToken));
            _projectRepository.RemoveVacancyToken(vacToken);
        }

        public VacancyToken GetVacancyToken(string vacTokenString)
        {
            Require.NotEmpty(vacTokenString, nameof(vacTokenString));
            var vacToken = _projectRepository.GetVacancyToken(vacTokenString);
            return vacToken;
        }
    }
}
