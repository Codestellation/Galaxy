using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.Domain
{
    [JsonConverter(typeof(FullPathConverter))]
    public struct FullPath : IEquatable<FullPath>
    {
        private readonly string _value;

        public bool IsEmpty => string.IsNullOrWhiteSpace(_value);

        public FullPath(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Must be not empty string", nameof(value));
            }
            if (!Path.IsPathRooted(value))
            {
                throw new ArgumentException("Must be a valid rooted path", nameof(value));
            }
            char[] invalidChars = Path.GetInvalidPathChars();

            if (value.IndexOfAny(invalidChars) >= 0)
            {
                string characters = string.Join(",", invalidChars.Select(x => $"'{x}'"));
                string message = $"Path '{value}' contains one or more invalid characters: {characters}";
                throw new ArgumentException(message, nameof(value));
            }
            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(FullPath other) => string.Equals(_value, other._value);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is FullPath path && Equals(path);
        }

        public override int GetHashCode() => _value?.GetHashCode() ?? 0;

        public static bool operator ==(FullPath left, FullPath right) => left.Equals(right);

        public static bool operator !=(FullPath left, FullPath right) => !left.Equals(right);

        public static explicit operator string(FullPath fullPath) => fullPath._value;

        public static explicit operator FullPath(string value) => new FullPath(value);
    }
}