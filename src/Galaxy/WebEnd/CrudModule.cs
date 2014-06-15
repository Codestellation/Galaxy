using Nancy.Security;

namespace Codestellation.Galaxy.WebEnd
{
    public abstract class CrudModule : ModuleBase
    {
        protected abstract CrudOperations SupportedOperations { get; }

        public CrudModule(string path) : base(path)
        {
            this.RequiresAuthentication();

            if ((SupportedOperations & CrudOperations.GetList) == CrudOperations.GetList)
            {
                Get["/", true] = (parameters, token) => ProcessRequest(() => GetList(parameters), token);
            }

            if ((SupportedOperations & CrudOperations.GetDetails) == CrudOperations.GetDetails)
            {
                Get["/details/{id}", true] = (parameters, token) => ProcessRequest(() => GetDetails(parameters), token);
            }

            if ((SupportedOperations & CrudOperations.GetCreate) == CrudOperations.GetCreate)
            {
                Get["/create", true] = (parameters, token) => ProcessRequest(() => GetCreate(parameters), token);
            }

            if ((SupportedOperations & CrudOperations.PostCreate) == CrudOperations.PostCreate)
            {
                Post["/create", true] = (parameters, token) => ProcessRequest(() => PostCreate(parameters), token);
            }

            if ((SupportedOperations & CrudOperations.GetEdit) == CrudOperations.GetEdit)
            {
                Get["/edit/{id}", true] = (parameters, token) => ProcessRequest(() => GetEdit(parameters), token);
            }

            if ((SupportedOperations & CrudOperations.PostEdit) == CrudOperations.PostEdit)
            {
                Post["/edit/{id}", true] = (parameters, token) => ProcessRequest(() => PostEdit(parameters), token);
            }

            if ((SupportedOperations & CrudOperations.GetDelete) == CrudOperations.GetDelete)
            {
                Get["/delete/{id}", true] = (parameters, token) => ProcessRequest(() => GetDelete(parameters), token);
            }

            if ((SupportedOperations & CrudOperations.PostDelete) == CrudOperations.PostDelete)
            {
                Post["/delete/{id}", true] = (parameters, token) => ProcessRequest(() => PostDelete(parameters), token);
            }
        }

        protected virtual object GetList(dynamic parameters)
        {
            throw new System.NotImplementedException();
        }

        protected virtual object GetDetails(dynamic parameters)
        {
            throw new System.NotImplementedException();
        }

        protected virtual object GetCreate(dynamic parameters)
        {
            throw new System.NotImplementedException();
        }

        protected virtual object PostCreate(dynamic parameters)
        {
            throw new System.NotImplementedException();
        }

        protected virtual object GetEdit(dynamic parameters)
        {
            throw new System.NotImplementedException();
        }

        protected virtual object PostEdit(dynamic parameters)
        {
            throw new System.NotImplementedException();
        }

        protected virtual object GetDelete(dynamic parameters)
        {
            throw new System.NotImplementedException();
        }

        protected virtual object PostDelete(dynamic parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}