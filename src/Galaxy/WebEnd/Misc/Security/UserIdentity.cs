using System;
using System.Collections.Generic;
using Nancy.Security;

namespace Codestellation.Galaxy.WebEnd.Misc.Security
{
    [Serializable]
    public class UserIdentity : IUserIdentity
    {
        private readonly string _userName;
        private readonly string[] _claims;

        public UserIdentity(string userName, bool isAdmin)
        {
            _userName = userName;
            _claims = isAdmin
                ? Security.Claims.Admin
                : Security.Claims.User;
        }

        public string UserName
        {
            get { return _userName; }
        }

        public IEnumerable<string> Claims
        {
            get { return _claims; }
        }
        
        public override string ToString()
        {
            return UserName;
        }
    }
}