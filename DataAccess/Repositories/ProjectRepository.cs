using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.NHibernate;
using Journalist;
using NHibernate.Linq;
using ProjectManagement.Domain;
using ProjectManagement.Infrastructure;
using UserManagement.Domain;
using UserManagement.Infrastructure;

namespace DataAccess.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ISessionProvider _sessionProvider;
        private readonly IUserRepository _userRepository;

        public ProjectRepository(ISessionProvider sessionProvider, IUserRepository userRepository)
        {
            Require.NotNull(sessionProvider, nameof(sessionProvider));
            Require.NotNull(userRepository, nameof(userRepository));
            _sessionProvider = sessionProvider;
            _userRepository = userRepository;
        }

        public int CreateProject(Project project)
        {
            Require.NotNull(project, nameof(project));

            var session = _sessionProvider.GetCurrentSession();
            var savedProjectId = (int)session.Save(project);
            return savedProjectId;
        }

        public int CreateVacancy(Vacancy vacancy)
        {
            Require.NotNull(vacancy, nameof(vacancy));

            var session = _sessionProvider.GetCurrentSession();
            var savedVacancyId = (int)session.Save(vacancy);
            return savedVacancyId;
        }

        public Project GetProject(int projectId)
        {
            Require.Positive(projectId, nameof(projectId));

            var session = _sessionProvider.GetCurrentSession();
            var project = session.Get<Project>(projectId);
            return project;
        }

        public List<Project> GetAllProjects(Func<Project, bool> predicate = null)
        {
            var session = _sessionProvider.GetCurrentSession();
            return predicate == null
                ? session.Query<Project>().ToList()
                : session.Query<Project>().Where(predicate).ToList();
        }

        public void RemoveProject(int projectId)
        {
            Require.Positive(projectId, nameof(projectId));

            var session = _sessionProvider.GetCurrentSession();
            var project = GetProject(projectId);
            project.ProjectTags = null;
            session.Delete(project);
        }

        public void RemoveVacancy(Project project, Vacancy vacancy, int userId)
        {
            Require.NotNull(vacancy, nameof(vacancy));
            Require.NotNull(project, nameof(project));


            Account account;
            if (userId != 0)
            {
                account = _userRepository.GetAccount(userId);
                if (account == null)
                {
                    throw new AccountNotFoundException("Account not found");
                }
                var portfolio = account.Profile.Portfolio.ToList();
                var p = new List<Vacancy>();
                p.Add(vacancy);
                vacancy.MemberId = 0;
                account.Profile.Portfolio = portfolio.Except(p);
                _userRepository.UpdateAccount(account);
            }

            var vacancies = project.Vacancies.ToList();
            var v = new List<Vacancy>();
            v.Add(vacancy);
            project.Vacancies = vacancies.Except(v);
            UpdateProject(project);

            vacancy.VacancyTags = null;

            var session = _sessionProvider.GetCurrentSession();
            session.Delete(vacancy);
        }

        public void AddMemberToProject(int userId, int vacancyId)
        {
            Require.Positive(userId, nameof(userId));
            Require.Positive(vacancyId, nameof(vacancyId));

            var account = _userRepository.GetAccount(userId);
            if (account == null)
            {
                throw new AccountNotFoundException("Account not found");
            }
            var vacancy = GetVacancy(vac => vac.VacancyId == vacancyId).Single();
            if (vacancy == null)
            {
                throw new VacancyNotFoundException("Vacancy not found");
            }
            var portfolio = account.Profile.Portfolio.ToList();
            portfolio.Add(vacancy);
            account.Profile.Portfolio = portfolio;
            _userRepository.UpdateAccount(account);
        }

        public void RemoveMemberFromProject(int vacancyId, int userId)
        {

            Require.Positive(vacancyId, nameof(vacancyId));

            var va = GetVacancy(item => item.VacancyId == vacancyId).SingleOrDefault();
            if (va == null)
            {
                throw new VacancyNotFoundException("Vacancy not found");
            }
            
            var account = _userRepository.GetAccount(userId);
            if (account == null)
            {
                throw new AccountNotFoundException("Account not found");
            }
            var vacancy = GetVacancy(vac => vac.VacancyId == vacancyId).Single();
            if (vacancy == null)
            {
                throw new VacancyNotFoundException("Vacancy not found");
            }
            var portfolio = account.Profile.Portfolio.ToList();
            var v = new List<Vacancy>();
            v.Add(vacancy);
            vacancy.MemberId = 0;
            account.Profile.Portfolio = portfolio.Except(v);
            _userRepository.UpdateAccount(account);         
        }

        public void UpdateProject(Project project)
        {
            Require.NotNull(project, nameof(project));

            var session = _sessionProvider.GetCurrentSession();
            session.Update(project);
        }

        public void UpdateVacancy(Vacancy vacancy)
        {
            Require.NotNull(vacancy, nameof(vacancy));

            var session = _sessionProvider.GetCurrentSession();
            session.Update(vacancy);
        }

        public List<Vacancy> GetVacancy(Func<Vacancy, bool> predicate = null)
        {
            var session = _sessionProvider.GetCurrentSession();
            return predicate == null
                ? session.Query<Vacancy>().ToList()
                : session.Query<Vacancy>().Where(predicate).ToList();
        }

        public void SaveVacancyToken(VacancyToken token)
        {
            Require.NotNull(token, nameof(token));

            var session = _sessionProvider.GetCurrentSession();
            session.Save(token);
        }

        public void RemoveVacancyToken(VacancyToken token)
        {
            Require.NotNull(token, nameof(token));

            var session = _sessionProvider.GetCurrentSession();
            session.Delete(token);
        }

        public VacancyToken GetVacancyToken(string vacTokenString)
        {
            Require.NotEmpty(vacTokenString, nameof(vacTokenString));

            var session = _sessionProvider.GetCurrentSession();
            var vacancyToken = session.Get<VacancyToken>(vacTokenString);
            return vacancyToken;
        }
    }
}
