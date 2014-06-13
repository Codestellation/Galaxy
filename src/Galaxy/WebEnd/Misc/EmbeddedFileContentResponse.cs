using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Nancy;

namespace Codestellation.Galaxy.WebEnd.Misc
{
    public class EmbeddedFileContentResponse : Response
    {
        public static readonly Assembly Assembly;
        private static readonly string ContentNamespace;
        private static readonly Dictionary<string, EmbeddedResource> Resources;
        private readonly static string LastModifiedString;
        private readonly static DateTime _modified;
        private static TimeSpan _maxAge;
        private static string _maxAgeString;

        static EmbeddedFileContentResponse()
        {
            Assembly = Assembly.GetExecutingAssembly();
            string ns = typeof(ModuleBase).Namespace;
            ContentNamespace = ns + ".Content.";

            Resources = Assembly
                .GetManifestResourceNames()
                .Where(resource => resource.StartsWith(ContentNamespace))
                .ToDictionary(resourceName => resourceName.Replace(ContentNamespace, string.Empty), resourceName => new EmbeddedResource(resourceName));

            var assmebly = Assembly.GetExecutingAssembly();
            _maxAge = TimeSpan.FromDays(10);
            _maxAgeString = _maxAge.TotalSeconds.ToString("max-age=##########", CultureInfo.InvariantCulture);


            _modified = File.GetCreationTime(assmebly.Location);
            LastModifiedString = _modified.ToHttpHeaderDate();
            
        }

        private EmbeddedFileContentResponse(ref EmbeddedResource resource, DateTime? ifModifiedSince)
        {
            ContentType = MimeTypes.GetMimeType(resource.ResourcePath);

            if (ifModifiedSince.HasValue && ifModifiedSince.Value <= _modified)
            {
                StatusCode = HttpStatusCode.NotModified;
                //X-Cache	HIT
                this.WithHeader("X-Cache", "HIT");
            }
            else
            {
                StatusCode = HttpStatusCode.OK;
                var content = resource.GetContent();
                content.Seek(0, SeekOrigin.Begin);
                Contents = content.CopyTo;
            }
            
            
            var now = DateTime.UtcNow;
            var expires = now.Add(_maxAge);

            this.WithHeader("ETag", resource.Etag);
            //Cache-Control	max-age=604800
            this.WithHeader("Cache-Control", _maxAgeString);
            //Date	Sat, 25 Jan 2014 10:30:21 GMT
            this.WithHeader("Date", now.ToHttpHeaderDate());
            //Expires	Sat, 01 Feb 2014 10:30:21 GMT            
            this.WithHeader("Expires", expires.ToHttpHeaderDate());
            //Last-Modified	Thu, 05 Dec 2013 17:04:39 GMT
            this.WithHeader("Last-Modified", LastModifiedString);
            
        }

        public static Response TryGetContent(NancyContext context, string resourceName)
        {
            var filename = Path.GetFileName(context.Request.Url.Path) ?? string.Empty;
            EmbeddedResource resource;

            if (!Resources.TryGetValue(filename, out resource))
            {
                return null;
            }

            return new EmbeddedFileContentResponse(ref resource, context.Request.Headers.IfModifiedSince);
        }
    }
}