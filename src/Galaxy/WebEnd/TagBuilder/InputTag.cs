using Codestellation.Galaxy.WebEnd.TagBuilder.Attributes;

namespace Codestellation.Galaxy.WebEnd.TagBuilder
{
    public class InputTag : Tag
    {
        public InputTag(string name, HtmlAttribute type) : base(name)
        {
            SetAttribute(type);
        }

        public sealed override bool AllowsCloseTag
        {
            get { return false; }
        }
    }
}