using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Codestellation.Galaxy.WebEnd.TagBuilder.Attributes;

namespace Codestellation.Galaxy.WebEnd.TagBuilder
{
    public class OptionTag : Tag
    {
        private bool _isSelected;

        public OptionTag()
            : base("option")
	    {
	    }

        public OptionTag Selected(bool isSelected)
        {
            _isSelected = isSelected;
            return this;
        }

        protected override void WriteAttributes(TextWriter writer)
        {
            if (_isSelected)
            {
                SetAttribute(HtmlAttribute.Selected);
            }

            base.WriteAttributes(writer);
        }
    }
}
