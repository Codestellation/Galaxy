using System;
using System.IO;

namespace Codestellation.Galaxy.Domain
{
    public class SpecialFolder
    {
        public string Purpose { get; private set; }

        public string FullPath { get; private set; }

        public SpecialFolder(string purpose, string fullPath)
        {
            if (string.IsNullOrWhiteSpace(purpose))
            {
                throw new ArgumentException("Must be neither null nor empty string", "purpose");
            }

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                throw new ArgumentException("Must be neither null nor empty string", "fullPath");
            }

            var invalidChars = Path.GetInvalidPathChars();

            if (fullPath.IndexOfAny(invalidChars) >= 0)
            {
                var message = string.Format("Path '{0}' contains one or more invalid characters {1}", fullPath, string.Join(",", invalidChars));
                throw new ArgumentException(message, "fullPath");
            }
            if (!Path.IsPathRooted(fullPath))
            {
                var message = string.Format("Path must be rooted, bus was '{0}'", fullPath);
                throw new ArgumentException(message, "fullPath");
            }

            Purpose = purpose;
            FullPath = fullPath;
        }

        protected bool Equals(SpecialFolder other)
        {
            return string.Equals(Purpose, other.Purpose);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SpecialFolder) obj);
        }

        public override int GetHashCode()
        {
            return Purpose.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Purpose, FullPath);
        }
    }
}