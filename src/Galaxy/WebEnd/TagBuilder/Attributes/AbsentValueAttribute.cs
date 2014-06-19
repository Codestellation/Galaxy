using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Codestellation.Galaxy.WebEnd.TagBuilder.Attributes
{
    /// <summary>
    /// parent class for attributes without value part
    /// sample: 'selected' attribute in <option> html-tag 
    /// <option value = "somevalue" selected></option>
    /// </summary>
    public class AbsentValueAttribute: HtmlAttribute
    {
        public AbsentValueAttribute(string name):
            base(name)
        {

        }

        public override void WriteTo(TextWriter writer)
        {
            writer.Write(Name);
        }

    }
}
