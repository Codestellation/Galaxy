using System;
using System.Collections.Generic;
using System.Reflection;

namespace Codestellation.Galaxy.ServiceManager.Helpers
{
    public static class DotNetVersionHelper
    {
        public static Version dotNetVersion35 = new Version(3, 5);
        public static Version dotNetVersion40 = new Version(4, 0);
        public static Version dotNetVersion45 = new Version(4, 5);

        public static Version[] allDotNetVersions = new Version[] 
        {
            dotNetVersion35,
            dotNetVersion40,
            dotNetVersion45
        };

        public static Dictionary<Version, string> dotNetNugetFolders =
            new Dictionary<Version, string> 
            { 
                {dotNetVersion35, "net35"}, 
                {dotNetVersion40, "net40"}, 
                {dotNetVersion45, "net45"}, 
            };
    }
}
