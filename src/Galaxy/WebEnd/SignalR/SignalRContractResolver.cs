using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json.Serialization;

namespace Codestellation.Galaxy.WebEnd.SignalR
{
    public class SignalRContractResolver : IContractResolver
    {
        private readonly Assembly _assembly;
        private readonly IContractResolver _camelCaseContractResolver;
        private readonly IContractResolver _defaultContractSerializer;

        public SignalRContractResolver()
        {
            _defaultContractSerializer = new DefaultContractResolver();
            _camelCaseContractResolver = new CamelCasePropertyNamesContractResolver();
            _assembly = typeof(Connection).Assembly;
        }

        public JsonContract ResolveContract(Type type)
        {
            if (IsSignalRRelatedType(type))
            {
                return _defaultContractSerializer.ResolveContract(type);
            }

            return _camelCaseContractResolver.ResolveContract(type);
        }

        private bool IsSignalRRelatedType(Type type)
        {
            if (type.Assembly.Equals(_assembly))
            {
                return true;
            }

            if (type.IsGenericType && type.GetGenericArguments().Any(x => x.Assembly.Equals(_assembly)))
            {
                return true;
            }

            return false;
        }
    }
}