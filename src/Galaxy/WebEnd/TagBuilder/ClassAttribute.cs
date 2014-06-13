using System.Collections.Generic;
using System.IO;

namespace Codestellation.Galaxy.WebEnd.TagBuilder
{
    public class ClassAttribute : HtmlAttribute
    {
        public readonly ISet<string> Values;

        public ClassAttribute(IEnumerable<string> classes) : base(Class)
        {
            Values = new HashSet<string>(classes);
        }

        protected override void WriteValue(TextWriter writer)
        {
            var isFirst = true;
            foreach (var value in Values)
            {
                if (!isFirst)
                {
                    writer.Write(" ");
                }

                writer.Write(value);
                isFirst = false;
            }
        }
    }
}