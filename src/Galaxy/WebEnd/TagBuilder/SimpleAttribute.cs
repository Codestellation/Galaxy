using System.IO;

namespace Codestellation.Galaxy.WebEnd.TagBuilder
{
    public class SimpleAttribute<TValue> : HtmlAttribute
    {
        public readonly TValue Value;

        public SimpleAttribute(string name, TValue value): base(name)
        {
            Value = value;
        }

        protected override void WriteValue(TextWriter writer)
        {
            writer.Write(Value);
        }
    }
}