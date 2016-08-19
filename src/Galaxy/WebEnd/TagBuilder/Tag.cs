using System.Collections.Generic;
using System.IO;
using Codestellation.Galaxy.WebEnd.TagBuilder.Attributes;
using Nancy.ViewEngines.Razor;

namespace Codestellation.Galaxy.WebEnd.TagBuilder
{
    public class Tag : IHtmlString
    {
        private readonly string _name;
        private readonly Dictionary<string, HtmlAttribute> _attributes;
        private readonly List<object> _contents;

        public Tag(string name)
        {
            _name = name;
            _attributes = new Dictionary<string, HtmlAttribute>();
            _contents = new List<object>();
        }

        public string ToHtmlString()
        {
            var writer = new StringWriter();

            writer.Write('<');
            writer.Write(_name);

            WriteAttributes(writer);

            if (AllowsCloseTag)
            {
                writer.Write('>');
                WriteContent(writer);
                WriteCloseTag(writer);
            }
            else
            {
                writer.Write('/');
                writer.Write('>');
            }

            return writer.ToString();
        }

        private void WriteContent(StringWriter writer)
        {
            foreach (var content in _contents)
            {
                writer.Write(content);
            }
        }

        protected virtual void WriteAttributes(TextWriter writer)
        {
            foreach (var attribute in Attributes)
            {
                writer.Write(" ");
                attribute.WriteTo(writer);
            }
        }

        public virtual bool AllowsCloseTag
        {
            get { return true; }
        }

        protected IEnumerable<HtmlAttribute> Attributes
        {
            get { return _attributes.Values; }
        }

        private void WriteCloseTag(StringWriter writer)
        {
            writer.Write("</");
            writer.Write(_name);
            writer.Write(">");
        }

        public override string ToString()
        {
            return ToHtmlString();
        }

        public Tag Classes(params string[] classes)
        {
            HtmlAttribute attribute;
            if (!_attributes.TryGetValue(HtmlAttribute.Class, out attribute))
            {
                attribute = new ClassAttribute(classes);
                _attributes[attribute.Name] = attribute;
            }
            else
            {
                foreach (var @class in classes)
                {
                    ((ClassAttribute)attribute).Values.Add(@class);
                }
            }
            return this;
        }

        public Tag Content(params object[] contents)
        {
            _contents.AddRange(contents);
            return this;
        }

        public Tag For<TValue>(TValue value)
        {
            return SetAttribute(new SimpleAttribute<TValue>(HtmlAttribute.For, value));
        }

        public Tag Name<TValue>(TValue value)
        {
            return SetAttribute(HtmlAttribute.CreateName(value));
        }

        public Tag Id<TValue>(TValue id)
        {
            return SetAttribute(HtmlAttribute.CreateId(id));
        }

        public Tag Placeholder<TValue>(TValue placeholder)
        {
            return SetAttribute(HtmlAttribute.CreatePlaceholder(placeholder));
        }

        public virtual Tag Value<TValue>(TValue value)
        {
            return SetAttribute(HtmlAttribute.CreateValue(value));
        }

        public virtual Tag Readonly()
        {
            return SetAttribute(HtmlAttribute.Readonly);
        }

        protected Tag SetAttribute(HtmlAttribute attribute)
        {
            _attributes[attribute.Name] = attribute;
            return this;
        }

        public Tag Rows<TValue>(TValue value)
        {
            return SetAttribute(HtmlAttribute.CreateRows(value));
        }
    }
}