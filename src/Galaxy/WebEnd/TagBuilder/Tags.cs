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
        }
        
        public static Tag Div()
        {
            return new Tag("div");
        }

        public static Tag Label()
        {
            return new Tag("label");
        }

        public static Tag TextArea()
        {
            return new Tag("textarea");
        }
    }
}