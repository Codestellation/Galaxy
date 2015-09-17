using System;

namespace Codestellation.Galaxy.Host.ConfigManagement
{
    public class ValidationError
    {
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
            Key = key;
            Message = message;
        }
    }
}