using Codestellation.Galaxy.WebEnd.TagBuilder.Attributes;

namespace Codestellation.Galaxy.WebEnd.TagBuilder
{
    public static class Tags
    {
        public static class Input
        {
            public static InputTag Text()
            {
                return new InputTag("input", HtmlAttribute.InputText);
            }

            public static InputTag Number()
            {
                return new InputTag("input", HtmlAttribute.InputNumber);
            }

            public static InputTag Password()
            {
                return new InputTag("input", HtmlAttribute.InputPassword);
            }

            public static CheckboxTag CheckBox()
            {
                return new CheckboxTag();
            }

            internal static SelectTag Select()
            {
                return new SelectTag();
            }

            internal static OptionTag Option()
            {
                return new OptionTag();
            }
        }

        public static Tag Div()
        {
            return new Tag("div");
        }

        public static Tag Label()
        {
            return new Tag("label");
        }

        public static TextAreaTag TextArea()
        {
            return new TextAreaTag();
        }
    }
}