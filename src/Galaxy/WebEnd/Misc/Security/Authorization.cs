using System;
using System.Security.Claims;
using System.Security.Principal;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Nancy;
using Nejdb;
using Nejdb.Queries;

namespace Codestellation.Galaxy.WebEnd.Misc.Security
{
    public static class Authorization
    {
        public static void Perform(NancyContext context, Repository repository)
        {
            var owinContext = context.ToOwinContext();

            var windowsPrincipal = owinContext.Request.User as WindowsPrincipal;
            if (!UserIsAuthenticated(windowsPrincipal))
            {
                return;
            }

            var username = GetDomainUsername(windowsPrincipal);

            var user = GetUser(repository.GetCollection<User>(), username);
            if (!UserIsRegistered(user))
            {
                return;
            }

            var userIdentity = CreateIdentityOf(user);
            context.CurrentUser = userIdentity;
        }

        private static bool UserIsAuthenticated(WindowsPrincipal principal)
        {
            return principal != null && principal.Identity.IsAuthenticated;
        }

        private static string GetDomainUsername(WindowsPrincipal principal)
        {
            var nameClaim = principal.FindFirst(ClaimTypes.Name);
            var domainUsername = nameClaim.Value.ToLowerInvariant();

            // Something like domain\username
            return domainUsername;
        }

        private static User GetUser(Collection users, string username)
        {
            var user = GetExistingUser(users, username);
            if (user != null)
            {
                return user;
            }

            if (ThereIsNoDomainUsers(users))
            {
                var initialAdmin = CreateDomainAdmin(users, username);
                return initialAdmin;
            }

            return null;
        }

        private static User GetExistingUser(Collection users, string username)
        {
            var byLogin = Criterions.Field<User, string>(x => x.Login, Criterions.Equals(username));
            var queryBuilder = new QueryBuilder(byLogin);

            using (var query = users.CreateQuery<User>(queryBuilder))
            using (var cursor = query.Execute())
            {
                return cursor.Count != 0 ? cursor[0] : null;
            }
        }

        private static bool ThereIsNoDomainUsers(Collection users)
        {
            var byDomain = Criterions.Field<User, bool>(x => x.IsDomain, Criterions.Equals(true));
            var queryBuilder = new QueryBuilder(byDomain);

            using (var query = users.CreateQuery<User>(queryBuilder))
            using (var cursor = query.Execute(QueryMode.Count))
            {
                return cursor.Count == 0;
            }
        }

        private static User CreateDomainAdmin(Collection users, string username)
        {
            using (var transaction = users.BeginTransaction())
            {
                var user = new User
                {
                    Login = username,
                    IsDomain = true,
                    IsAdmin = true
                };

                users.Save(user, false);
                transaction.Commit();

                return user;
            }
        }

        private static bool UserIsRegistered(User user)
        {
            return user != null;
        }

        private static UserIdentity CreateIdentityOf(User user)
        {
            return new UserIdentity(user.Login, user.IsAdmin);
        }
    }
}