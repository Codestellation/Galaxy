using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Nancy.Authentication.Forms;
using Nancy.Extensions;
using Nejdb;
using Nejdb.Queries;

namespace Codestellation.Galaxy.WebEnd
{
    public class LoginModule : ModuleBase
    {
        private readonly UserDatabase _userDatabase;
        Collection _users;

        public LoginModule(Repository repository, UserDatabase userDatabase)
            : base("login")
        {
            _userDatabase = userDatabase;
            _users = repository.GetCollection<User>();

            Get["/"] = parameters => View["Login"];

            Get["/login"] = parameters => ProcessLogin();

            Get["/logout"] = parameters => this.LogoutAndRedirect("~/login");

            Post["/"] = parameters => ProcessLogin();
        }

        private object ProcessLogin()
        {
            var user = GetUser((string)Request.Form.Login, (string)Request.Form.Password);

            if (user == null)
            {
                return Context.GetRedirect("~/login?error=true&username=" + (string)Request.Form.Login);
            }

            var identity = new UserIdentity(user);

            _userDatabase.PutToSession(Context, identity);

            return this.LoginAndRedirect(identity.Guid, identity.Expiry);
        }

        private User GetUser(string login, string password)
        {
            var byName = Criterions.Field<User, string>(x => x.Login, Criterions.Equals(login));
            var byAge = Criterions.Field<User, string>(x => x.Password, Criterions.Equals(password));

            var andCriterion = Criterions.And(byName, byAge);
            var builder = new QueryBuilder(andCriterion);

            using (var query = _users.CreateQuery<User>(builder))
            using (var cursor = query.Execute())
            {
                if (cursor.Count == 0) return null;
                return cursor[0];
            }
        }
    }
}