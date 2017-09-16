using System;
using System.Collections.Generic;
using ProjectManagement.Domain;

namespace ProjectManagement.Infrastructure
{
    public interface IProjectRepository
    {
        Project GetProject(int projectId);

        int CreateProject(Project request);

        int CreateVacancy(Vacancy vacancy);

        List<Project> GetAllProjects(Func<Project, bool> predicate = null);

        void RemoveProject(int projectId);

        void AddMemberToProject(int userId, int vacancyId);

        void RemoveMemberFromProject(int vacancyId, int userId);

        void UpdateProject(Project project);

        void UpdateVacancy(Vacancy vacancy);

        void RemoveVacancy(Project project, Vacancy vacancy, int userId);

        List<Vacancy> GetVacancy(Func<Vacancy, bool> predicate = null);

        void SaveVacancyToken(VacancyToken token);

        void RemoveVacancyToken(VacancyToken token);

        VacancyToken GetVacancyToken(string vacTokenString);
    }
}
