using System;
using System.IO;
using System.Reflection;
using System.Text;
using Nancy;
using Nancy.IO;

namespace Codestellation.Galaxy.WebEnd.Misc
{
    public class ForbidenResponse : Response
    {
        public ForbidenResponse()
        {
            ContentType = "text/html";

            Contents = s =>
                {
                    using (var writer = new StreamWriter(new UnclosableStreamWrapper(s), Encoding.UTF8))
                    {
                        writer.Write(LoadResource("403.html"));
                    }
                };
        }

        private static string LoadResource(string filename)
        {
            var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(String.Format("Codestellation.Galaxy.WebEnd.Views.ErrorHandling.Resources.{0}", filename));

            if (resourceStream == null)
            {
                return string.Empty;
            }

            using (var reader = new StreamReader(resourceStream))
            {
                return reader.ReadToEnd();
            }
        }


    }
}