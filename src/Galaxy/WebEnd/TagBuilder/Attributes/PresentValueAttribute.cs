using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codestellation.Galaxy.WebEnd.TagBuilder.Attributes
{
    /// <summary>
    /// parent class for attributes with value part
    /// someattribute = "value"
    /// </summary>
    public abstract class PresentValueAttribute: HtmlAttribute
    {
        public PresentValueAttribute(string name):
            base(name)
        {

        }

        public override void WriteTo(TextWriter writer)
        {
            writer.Write(Name);
            writer.Write('=');
            writer.Write('"');

            WriteValue(writer);

            writer.Write('"');
        }

        protected abstract void WriteValue(TextWriter writer);
    }
}
