using System;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;

namespace Codestellation.Galaxy.Infrastructure
{
    public class UserDatabase : IUserMapper
    {
        public const string UserKey = "User";
        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            return (UserIdentity) context.Request.Session[UserKey];
        }

        public void PutToSession(NancyContext context, UserIdentity userIdentity)
        {
            context.Request.Session[UserKey] = userIdentity;
        }
    }
}
