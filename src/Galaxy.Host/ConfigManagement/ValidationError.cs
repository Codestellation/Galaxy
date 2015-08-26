using System;

namespace Codestellation.Galaxy.Host.ConfigManagement
{
    public class ValidationError
    {
        private readonly string _key;
        private readonly string _message;
        public readonly string Key;
        public readonly string Message;

        public ValidationError(string key, string message)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Should be neither null nor empty string", "key");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Should be neither null nor empty string", "message");
            }
            _key = key;
            _message = message;
        }
    }
}