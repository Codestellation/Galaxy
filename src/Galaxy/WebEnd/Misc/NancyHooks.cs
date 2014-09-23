using Castle.Windsor;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.WebEnd.Misc.Security;
using Nancy;

namespace Codestellation.Galaxy.WebEnd.Misc
{
    public static class NancyHooks
    {
        public static Response AuthorizeUser(IWindsorContainer container, NancyContext context)
        {
            var repository = container.Resolve<Repository>();
            Authorization.Perform(context, repository);
            return null;
        }
    }
}