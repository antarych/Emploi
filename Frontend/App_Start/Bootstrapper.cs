using System;
using System.Configuration;
using SimpleInjector;
using System.Web.Http;
using Common;
using DataAccess.NHibernate;
using DataAccess.Repositories;
using FileManagement;
using Frontend.App_Data;
using OrganizationManagement.Application;
using OrganizationManagement.Domain;
using OrganizationManagement.Infastructure;
using ProjectManagement.Application;
using ProjectManagement.Domain;
using ProjectManagement.Infrastructure;
using SimpleInjector.Integration.WebApi;
using TagManagement.Application;
using TagManagement.Domain;
using UserManagement.Application;
using UserManagement.Domain;
using UserManagement.Domain.Mailing.Domain;
using UserManagement.Domain.Mailing.Infrastructure;
using UserManagement.Infrastructure;
using IUserRepository = UserManagement.Infrastructure.IUserRepository;

namespace Frontend.App_Start
{
    public class Bootstrapper
    {
        public void Setup()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            RegisterSettings(container);
            SetupDependencies(container);
            RegisterMailing(container);
            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }

        private void SetupDependencies(Container container)
        {
            container.Register<IUserRepository>(() => container.GetInstance<UserRepository>(), Lifestyle.Singleton);
            container.Register<IProjectRepository, ProjectRepository>();
            container.Register<IProjectManager, ProjectManager>();
            container.Register<IOrganizationRepository, OrganizationRepository>();
            container.Register<IOrganizationManager, OrganizationManager>();
            container.Register<IUserManager, UserManager>();
            container.Register<ISessionProvider, SessionProvider>();
            container.Register<IConfirmationRepository, ConfirmationRepository>();
            container.Register<IMailingRepository, MailingRepository>();
            container.Register<ITagManager, TagManager>();
            container.Register<ITagRepository, TagRepository>();
            container.Register<IAuthorizer>(() => new Authorizer(
                    TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["Authorizer.TokenLifeTimeInSeconds"])),
                    container.GetInstance<IUserRepository>(), container.GetInstance<IConfirmationRepository>()),
                Lifestyle.Singleton);
            container.Register<IFileManager, FileManager>(Lifestyle.Singleton);
        }

        private static void RegisterSettings(Container container)
        {
            var settings = ConfigurationManager.AppSettings;
            container.Register(() => SettingsReader.ReadFileStorage(settings), Lifestyle.Singleton);

        }

        private static void RegisterMailing(Container container)
        {
            container.Register<IMailingService, MailingService>();
            container.Register<IMailer, Mailer>();
            container.Register<MailSender>();
            var sender = container.GetInstance<MailSender>();
            sender.StartSending();
        }

    }
}