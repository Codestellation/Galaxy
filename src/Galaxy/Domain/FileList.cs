using System;

namespace Codestellation.Galaxy.Domain
{
    public class FileList
    {
        private readonly string[] _patterns;

        public FileList(string[] patterns)
        {
            _patterns = patterns;
        }

        public override string ToString()
        {
            if (_patterns == null || _patterns.Length == 0)
            {
                return string.Empty;
            }
            return string.Join(Environment.NewLine, _patterns);
        }
    }
}