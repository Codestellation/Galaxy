using System.Reflection;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.WebEnd;
using Codestellation.Galaxy.WebEnd.Misc;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.ErrorHandling;
using Nancy.Session;
using Nancy.TinyIoc;
using Nancy.ViewEngines;
using Nancy.ViewEngines.Razor;
using Nejdb;

namespace Codestellation.Galaxy
{
    public class Bootstrapper : DefaultNancyBootstrapper
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

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            
            container.Register<Repository>().AsSingleton();
            container.Register<IRazorConfiguration,RazorConfiguration>().AsSingleton();
            container.Register<IStatusCodeHandler,ForbiddenErrorHandler>().AsSingleton();
            container.Register<IUserMapper, UserDatabase>().AsSingleton();

            container.Register<DashBoard>().AsSingleton();
            container.Register<PackageVersionCache>().AsSingleton();

            //This should be the assembly your views are embedded in
            ResourceViewLocationProvider.RootNamespaces.Add(Assembly, ViewsNamespace);
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            var formsAuthConfiguration = new FormsAuthenticationConfiguration { RedirectUrl = "~/login", UserMapper = container.Resolve<IUserMapper>()};
            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);

            CookieBasedSessions.Enable(pipelines);

            var repository = container.Resolve<Repository>();
            repository.Start();
            
            CreateDefaultUser(repository);
            LoadOptions(container, repository);
            FillDashBoard(container, repository);

            container.Resolve<PackageVersionCache>().Start();

            base.ApplicationStartup(container, pipelines);
        }

        private void LoadOptions(TinyIoCContainer container, Repository repository)
        {
            var optionCollection = repository.GetCollection<Options>();
            using (var query = optionCollection.CreateQuery<Options>())
            using (var cursor = query.Execute())
            {
                var options = cursor.Count == 0 ? new Options() : cursor.Current;
                container.Register(options);
            }
        }

        private void FillDashBoard(TinyIoCContainer container,  Repository repository)
        {
            var dashBoard = container.Resolve<DashBoard>();

            using(var query = repository.GetCollection<NugetFeed>().CreateQuery<NugetFeed>())
            using(var cursor = query.Execute())
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
                    Password = "admin",
                    DisplayName = "Temp Admin"
                };
                users.Save(user, false);
                tx.Commit();
            }
        }
    }
}
