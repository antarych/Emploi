using System;
using System.Collections.Generic;
using ProjectManagement.Domain;

namespace ProjectManagement.Application
{
    public interface IProjectManager
    {
        Project GetProject(int projectId);

        int CreateProject(CreateProjectRequest request);

        void UpdateProject(Project project);

        IEnumerable<Project> GetProjects(Func<Project, bool> predicate = null);

        void RemoveProject(int projectId);

        void AddMemberToProject(int vacancyId, int userId);

        void RemoveMemberFromProject(int vacancyId, int userId);

        int CreateVacancy(int projectId, Vacancy vacancy);

        void UpdateVacancy(Vacancy vacancy);

        void RemoveVacancy(Project project, Vacancy vacancy, int userId);

        void AddTagsToProject(IList<string> tags, int projectId);

        void AddTagsToVacancy(IList<string> tags, int vacancyId);

        List<Vacancy> GetVacancy(Func<Vacancy, bool> predicate = null);

        IList<Project> GetProjectsByTags(IList<string> tags);

        IList<Vacancy> GetVacanciesByTags(IList<string> tags);

        VacancyToken GenerateVacancyToken(int vacancyId);

        void RemoveVacancyToken(VacancyToken vacToken);

        VacancyToken GetVacancyToken(string vacTokenString);
    }
}
