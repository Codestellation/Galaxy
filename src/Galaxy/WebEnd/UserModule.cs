using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.WebEnd.Models;
using Nancy.ModelBinding;
using Nancy.Responses;
using Nancy.Security;
using Nejdb;
using Nejdb.Bson;
using Nejdb.Queries;

namespace Codestellation.Galaxy.WebEnd
{
    public class UserModule : CrudModule
    {
        private readonly Collection _users;

        public UserModule(Repository repository)
            : base("user")
        {
            this.RequiresClaims(new[] { "Admin" });
            
            _users = repository.GetCollection<User>();

            Get["/check-login", true] = (parameters, token) => ProcessRequest(CheckLogin, token);
        }

        private JsonResponse CheckLogin()
        {
            if (Request.Query.ContainsKey("Login"))
            {
                return new JsonResponse(true, new DefaultJsonSerializer());
            }

            string login = Request.Query.Login;

            var byName = Criterions.Field<User, string>(x => x.Login, Criterions.Equals(login));
                
            var builder = new QueryBuilder(byName);

            using (var query = _users.CreateQuery<User>(builder))
            using (var cursor = query.Execute(QueryMode.Count))
            {
                if (cursor.Count > 0)
                {
                    var message = string.Format("Login '{0}' already exists", login);
                    return new JsonResponse(message, new DefaultJsonSerializer());
                }
            }

            return new JsonResponse(true, new DefaultJsonSerializer());
        }

        protected override CrudOperations SupportedOperations
        {
            get { return CrudOperations.GetList | CrudOperations.GetCreate | CrudOperations.PostCreate | CrudOperations.GetEdit | CrudOperations.PostEdit | CrudOperations.PostDelete; }
        }

        protected override object GetList(dynamic parameters)
        {
            var queryResults = _users.PerformQuery<User>();

            return View["List", queryResults];
        }

        protected override object GetCreate(dynamic parameters)
        {
            return View["Edit", new UserModel()];
        }

        protected override object PostCreate(dynamic parameters)
        {
            var userModel = this.Bind<UserModel>();
            var user = userModel.ToUser();

            using (var tx = _users.BeginTransaction())
            {
                _users.Save(user, false);
                tx.Commit();
            }
            return new RedirectResponse("/user");
        }

        protected override object GetEdit(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var user = _users.Load<User>(id);

            return View["Edit", new UserModel(user)];
        }

        protected override object PostEdit(dynamic parameters)
        {
            var userModel = this.Bind<UserModel>();
            var id = new ObjectId(parameters.id);
            var user = _users.Load<User>(id);

            userModel.Update(user);

            using (var tx = _users.BeginTransaction())
            {
                _users.Save(user, false);
                tx.Commit();
            }
            return new RedirectResponse("/user");
        }

        protected override object PostDelete(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            _users.Delete(id);

            return new RedirectResponse("/user");
        }
    }
}