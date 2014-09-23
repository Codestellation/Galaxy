using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Notifications;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.WebEnd;
using Codestellation.Galaxy.WebEnd.Misc;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Windsor;
using Nancy.ErrorHandling;
using Nancy.ViewEngines;
using Nancy.ViewEngines.Razor;
using Nejdb;

namespace Codestellation.Galaxy
{
    public class Bootstrapper : WindsorNancyBootstrapper
    {
        private static readonly Assembly Assembly;
        private static readonly string ViewsNamespace;

        static Bootstrapper()
        {
            Assembly = Assembly.GetExecutingAssembly();
            var ns = typeof(ModuleBase).Namespace;
            ViewsNamespace = ns + ".Views";
            StaticConfiguration.DisableErrorTraces = false;
        }

        protected override void ConfigureConventions(Nancy.Conventions.NancyConventions nancyConventions)
        {
            nancyConventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat(ViewsNamespace, "/", viewName));
            nancyConventions.StaticContentsConventions.Add(EmbeddedFileContentResponse.TryGetContent);

            base.ConfigureConventions(nancyConventions);
        }

        protected override void ConfigureApplicationContainer(IWindsorContainer container)
        {
            container.Install(FromAssembly.This());
            
            container.Register(
                Component
                    .For<Repository>()
                    .LifestyleSingleton(),
                Component
                    .For<IRazorConfiguration>()
                    .Named("DefaultRazorConfiguration")
                    .IsDefault()
                    .ImplementedBy<RazorConfiguration>()
                    .LifestyleSingleton(),

                Component
                    .For<IStatusCodeHandler>()
                    .ImplementedBy<ForbiddenErrorHandler>()
                    .Named("DefaultErrorHandler")
                    .IsDefault()
                    .LifestyleSingleton(),

                Component
                    .For<DashBoard>()
                    .LifestyleSingleton(),
                Component.
                    For<PackageVersionCache>()
                    .LifestyleSingleton(),

                Component
                    .For<TaskBuilder>()
                    .LifestyleTransient(),

                Component
                    .For<OperationBuilder>()
                    .LifestyleTransient(),

                Component
                    .For<Notifier>()
                    .LifestyleSingleton()
                );
            base.ConfigureApplicationContainer(container);

            //This should be the assembly your views are embedded in
            ResourceViewLocationProvider.RootNamespaces.Add(Assembly, ViewsNamespace);
        }

        protected override void ApplicationStartup(IWindsorContainer container, IPipelines pipelines)
        {
            var repository = container.Resolve<Repository>();
            repository.Start();

            EnableAuthorization(container, pipelines);

            CreateDefaultUser(repository);
            LoadOptions(container, repository);
            FillDashBoard(container, repository);

            container.Resolve<PackageVersionCache>().Start();

            base.ApplicationStartup(container, pipelines);
        }

        private void EnableAuthorization(IWindsorContainer container, IPipelines pipelines)
        {
            pipelines.BeforeRequest.AddItemToStartOfPipeline(context => NancyHooks.AuthorizeUser(container, context));
        }


        private void LoadOptions(IWindsorContainer container, Repository repository)
        {
            var optionCollection = repository.GetCollection<Options>();
            using (var query = optionCollection.CreateQuery<Options>())
            using (var cursor = query.Execute())
            {
                var options = cursor.Count == 0 ? new Options() : cursor.Current;
                container.Register(
                    Component
                    .For<Options>()
                    .Instance(options));
            }
        }

        private void FillDashBoard(IWindsorContainer container, Repository repository)
        {
            var dashBoard = container.Resolve<DashBoard>();

            using (var query = repository.GetCollection<NugetFeed>().CreateQuery<NugetFeed>())
            using (var cursor = query.Execute())
            {
                foreach (var feed in cursor)
                {
                    dashBoard.AddFeed(feed);
                }
            }

            using (var query = repository.GetCollection<Deployment>().CreateQuery<Deployment>())
            using (var cursor = query.Execute())
            {
                foreach (var deployment in cursor)
                {
                    dashBoard.AddDeployment(deployment);
                }
            }

        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get { return NancyInternalConfiguration.WithOverrides(OnConfigurationBuilder); }
        }

        private void OnConfigurationBuilder(NancyInternalConfiguration x)
        {
            x.ViewLocationProvider = typeof(ResourceViewLocationProvider);
        }

        private void CreateDefaultUser(Repository repository)
        {
            var users = repository.GetCollection<User>();

            using (var query = users.CreateQuery<User>())
            using (var cursor = query.Execute(QueryMode.Count))
            {
                if (cursor.Count > 0) return;
            }

            using (var tx = users.BeginTransaction())
            {
                var user = new User
                {
                    IsAdmin = true,
                    Login = "admin",
                };
                users.Save(user, false);
                tx.Commit();
            }
        }
    }
}
