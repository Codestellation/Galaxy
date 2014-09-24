using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Codestellation.Galaxy.WebEnd.Misc
{
    public class EmbeddedResourceList
    {
        private static readonly Dictionary<string, EmbeddedResource> Resources;
        private static readonly Assembly Assembly;

        public static readonly TimeSpan MaxAge;
        public static readonly string MaxAgeString;

        public readonly static string LastModifiedString;
        public readonly static DateTime Modified;

        static EmbeddedResourceList()
        {
            Assembly = Assembly.GetExecutingAssembly();
            Resources = Assembly
                .GetManifestResourceNames()
                .ToDictionary(resourceName =>resourceName.Replace("Codestellation.Galaxy.WebEnd.Content.", string.Empty), resourceName => new EmbeddedResource(Assembly, resourceName));

            

            MaxAge = TimeSpan.FromDays(10);
            MaxAgeString = MaxAge.TotalSeconds.ToString("max-age=##########", CultureInfo.InvariantCulture);

            Modified = File.GetCreationTime(Assembly.Location);
            LastModifiedString = Modified.ToHttpHeaderDate();
        }


        public static bool TryGetResource(string path, out EmbeddedResource resource)
        {
            var filename = Path.GetFileName(path) ?? string.Empty;

            return Resources.TryGetValue(filename, out resource);
        }
    }
}