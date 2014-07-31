using System;
using System.Collections.Generic;
using Codestellation.Galaxy.Domain;
using Nancy.Security;

namespace Codestellation.Galaxy.Infrastructure
{
    [Serializable]
    public class UserIdentity : IUserIdentity
    {
        private static readonly string[] UserClaims = new string[0];

        private static readonly string[] AdminClaims = { "Admin" };
        private string _userName;

        public UserIdentity(User user)
        {
            UserName = user.DisplayName;

            Claims = user.IsAdmin ? AdminClaims : UserClaims;

            Expiry = DateTime.UtcNow.AddDays(7);
            Guid = Guid.NewGuid();

        }

        public string UserName
        {
            get { return _userName ?? "<Unknown User>"; }
            set { _userName = value; }
        }

        public IEnumerable<string> Claims { get; set; }
        public Guid Guid { get; private set; }
        public DateTime? Expiry { get; private set; }

        public override string ToString()
        {
            return UserName;
        }
    }
}
