using System;
using System.Collections.Generic;
using System.Linq;

namespace Codestellation.Galaxy.Host.ConfigManagement
{
    public class ConsulConfig
    {
        private readonly Type _configType;
        private readonly ConfigLoader _loader;
        private readonly string _consulName;
        private readonly HashSet<ConfigElement> _elements;

        public ConsulConfig(Type configType, ConfigLoader loader, string consulName)
        {
            if (configType == null)
            {
                throw new ArgumentNullException("configType");
            }

            if (loader == null)
            {
                throw new ArgumentNullException("loader");
            }

            if (string.IsNullOrWhiteSpace(consulName))
            {
                var message = "Should be neither null nor empty string.";
                throw new ArgumentException(message, "consulName");
            }

            _configType = configType;
            _loader = loader;
            _consulName = consulName;
            _elements = ParseKeys();
            ValidationResult = new ValidationResult();
        }

        private HashSet<ConfigElement> ParseKeys()
        {
            var props = _configType
                .GetProperties()
                .Select(x => new ConfigElement(_consulName, x));
            return new HashSet<ConfigElement>(props);
        }

        public IEnumerable<ConfigElement> Elements
        {
            get { return _elements; }
        }

        public ValidationResult ValidationResult { get; private set; }

        public void Load()
        {
            foreach (var element in _elements)
            {
                _loader.Load(element);

                if (element.IsMissed || !element.IsValid)
                {
                    var error = new ValidationError(element.Key, element.Message);
                    ValidationResult.AddError(error);
                }
            }
        }

        public object BuildConfig()
        {
            var config = Activator.CreateInstance(_configType);
            foreach (var element in Elements)
            {
                element.Populate(config);
            }
            return config;
        }
    }
}