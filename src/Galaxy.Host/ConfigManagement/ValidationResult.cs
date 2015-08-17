using System;
using System.Collections.Generic;
using System.Linq;

namespace Codestellation.Galaxy.Host.ConfigManagement
{
    public class ValidationResult
    {
        private readonly List<ConfigElement> _errors;

        public ValidationResult()
        {
            _errors = new List<ConfigElement>();
        }

        public void AddError(ConfigElement error)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }
            _errors.Add(error);
        }

        public bool IsValid
        {
            get { return _errors.Count == 0; }
        }

        public IReadOnlyCollection<ConfigElement> Errors
        {
            get { return _errors; }
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Errors.Select(x => string.Format("Property '{0}' has error: {1}", x.Property, x.Message)));
        }
    }
}