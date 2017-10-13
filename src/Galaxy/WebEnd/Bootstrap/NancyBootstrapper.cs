using System;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.WebEnd.Misc;
using Codestellation.Pulsar;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Windsor;
using Nancy.Conventions;
using Nancy.ErrorHandling;
using Nancy.ViewEngines;
using Nancy.ViewEngines.Razor;
using Nejdb;

namespace Codestellation.Galaxy.WebEnd.Bootstrap
{
    public class NancyBootstrapper : WindsorNancyBootstrapper
    {
        private readonly IWindsorContainer _container;
        private static readonly Assembly Assembly;
        private static readonly string ViewsNamespace;

        static NancyBootstrapper()
        {
            Assembly = Assembly.GetExecutingAssembly();
            var ns = typeof(ModuleBase).Namespace;
            ViewsNamespace = ns + ".Views";
            StaticConfiguration.DisableErrorTraces = false;
        }

        public NancyBootstrapper(IWindsorContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            _container = container;
        }

        protected override IWindsorContainer GetApplicationContainer()
        {
            return _container;
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat(ViewsNamespace, "/", viewName));

            base.ConfigureConventions(nancyConventions);
        }

        protected override void ConfigureApplicationContainer(IWindsorContainer container)
        {
            container.Register(
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

            container.Resolve<ISchedulerController>().Start();

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
                if (cursor.Count > 0)
                {
                    return;
                }
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