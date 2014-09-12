using System;
using System.Diagnostics;
using Codestellation.Quarks.StringUtils;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.StringUtils
{
    [TestFixture]
    public class TemplateExpanderTests
    {
        public class PropertyObject
        {
            public PropertyObject(string some)
            {
                Text = some;
            }

            public string Text { get; set; }

            public int Count
            {
                get { return 4; }
            }
        }

        private TemplateExpander _template;

        [Test]
        [TestCase("{Text}", "some")]
        [TestCase("{Text}.bar", "some.bar")]
        [TestCase("foo.{Text}.bar", "foo.some.bar")]
        [TestCase("foo.{Text}", "foo.some")]
        [TestCase("foo.bar", "foo.bar")]
        [TestCase("foo.{Text}{Count}", "foo.some4")]
        public void Should_render_template(string templateContent, string expected)
        {
            _template = new TemplateExpander(templateContent);
            var message = new PropertyObject("some");

            //preheating.
            string actual = _template.Render(message);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int count = 1000000;

            for (int i = 0; i < count; i++)
            {
                actual = _template.Render(message);
            }
            stopwatch.Stop();

            Console.WriteLine("{0} - {1}", templateContent, stopwatch.Elapsed);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}