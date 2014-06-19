using System.IO;

namespace Codestellation.Galaxy.WebEnd.TagBuilder.Attributes
{
    public class SimpleAttribute<TValue> : PresentValueAttribute
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