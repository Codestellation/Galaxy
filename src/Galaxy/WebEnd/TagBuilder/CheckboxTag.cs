using System.IO;
using Codestellation.Galaxy.WebEnd.TagBuilder.Attributes;

namespace Codestellation.Galaxy.WebEnd.TagBuilder
{
    public class CheckboxTag : InputTag
    {
        private bool _isChecked;

        public CheckboxTag() : base("input", HtmlAttribute.InputCheckbox)
        {

        }

        public CheckboxTag Checked(bool isChecked)
        {
            _isChecked = isChecked;
            return this;
        }

        protected override void WriteAttributes(TextWriter writer)
        {
            if (_isChecked)
            {
                SetAttribute(HtmlAttribute.Checked);
            }

            base.WriteAttributes(writer);
        }
    }
}