namespace Codestellation.Galaxy.WebEnd.TagBuilder
{
    public class TextAreaTag : Tag
    {
        public TextAreaTag()
            : base("textarea")
        {

        }

        public override Tag Value<TValue>(TValue value)
        {
            Content(value);
            return this;
        }
    }
}