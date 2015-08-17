using System;
using System.Reflection;

namespace Codestellation.Galaxy.Host.ConfigManagement
{
    public class ConfigElement
    {
        private object _value;
        private byte[] _rawValue;
        private bool _isMissed;
        private bool _isValid;

        public ConfigElement(string consulName, PropertyInfo property)
        {
            Property = property;
            if (string.IsNullOrWhiteSpace(consulName))
            {
                throw new ArgumentException("Should be neither null nor empty string", "consulName");
            }
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            Key = property.Name.ToLowerInvariant();
            Path = string.Format("{0}.{1}", consulName, Key).Replace('.', '/');
        }

        public PropertyInfo Property { get; private set; }

        public string Key { get; private set; }

        public string Path { get; private set; }

        public byte[] RawValue
        {
            get
            {
                EnsureState();
                return _rawValue;
            }
            private set { _rawValue = value; }
        }

        public object Value
        {
            get
            {
                EnsureState();
                return _value;
            }
            private set { _value = value; }
        }

        public bool IsMissed
        {
            get
            {
                EnureLoaded();
                return _isMissed;
            }
            private set { _isMissed = value; }
        }

        public bool IsLoaded { get; private set; }

        public Type Type
        {
            get { return Property.PropertyType; }
        }

        public bool IsValid
        {
            get
            {
                EnsureState();
                return _isValid;
            }
            private set { _isValid = value; }
        }

        public string Message { get; private set; }

        public void ValueMissed()
        {
            IsLoaded = true;
            IsMissed = true;
            Message = "Value is missed";
        }

        public void ValueInvalid(byte[] raw, string message)
        {
            IsLoaded = true;
            IsMissed = false;
            IsValid = false;

            RawValue = raw;
            Message = message;
        }

        public void ValueFound(byte[] rawValue, object value)
        {
            IsValid = true;
            IsLoaded = true;
            Value = value;
            RawValue = rawValue;
        }

        private void EnsureState()
        {
            EnureLoaded();
            if (IsMissed)
            {
                var message = string.Format("Could not access property before because key {0} at path {1} missed in consul. Please, ensure configurations.", Key, Path);
                throw new InvalidOperationException(message);
            }
        }

        private void EnureLoaded()
        {
            if (!IsLoaded)
            {
                var message = string.Format("Could not access property before element {0} is loaded. User ConfigElement.Load() first.", Key);
                throw new InvalidOperationException(message);
            }
        }

        public void Populate(object config)
        {
            Property.SetValue(config, Value);
        }
    }
}