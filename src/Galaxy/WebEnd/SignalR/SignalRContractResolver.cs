﻿using System;
using System.Reflection;
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
            _assembly = typeof(Microsoft.AspNet.SignalR.Infrastructure.Connection).Assembly;
        }

        public JsonContract ResolveContract(Type type)
        {
            if (type.Assembly.Equals(_assembly))
            {
                return _defaultContractSerializer.ResolveContract(type);
            }

            return _camelCaseContractResolver.ResolveContract(type);
        }
    }
}