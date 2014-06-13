using System.Collections.Generic;
using Nancy.ViewEngines.Razor;
using Nejdb;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Misc
{
    public class RazorConfiguration : IRazorConfiguration
    {
        private string[] _assemblyNames;
        private string[] _namespaces;

        public RazorConfiguration()
        {
            _assemblyNames = new[] {typeof (ObjectId).Assembly.FullName, typeof (Collection).Assembly.FullName};
            _namespaces = new[] {typeof (ObjectId).Namespace, GetType().Namespace};
        }
        public IEnumerable<string> GetAssemblyNames()
        {
            return _assemblyNames;
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            return _namespaces;
        }

        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }
    }
}