using System;
using System.Globalization;
using System.Net;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.StaticFiles.Infrastructure;
using Owin;

namespace Codestellation.Galaxy.WebEnd.Bootstrap
{
    public static class OwinExtensions
    {
        private static readonly string MaxAgeString = TimeSpan
            .FromDays(10)
            .TotalSeconds
            .ToString("max-age=##########", CultureInfo.InvariantCulture);

        private static readonly string[] AllowedToAnonymous = { ".js", ".css", ".woff", ".ttf", ".svg" };

        public static void ServeEmbeddedFiles(this IAppBuilder app)
        {
            var fileSystem = new EmbeddedResourceFileSystem(Web.Namespace + ".Static");

            SharedOptions sharedOptions = new SharedOptions
            {
                FileSystem = fileSystem,
                RequestPath = new PathString("/static")
            };
            var staticOptions = new StaticFileOptions(sharedOptions)
            {
                ServeUnknownFileTypes = true,
                OnPrepareResponse = context => context.OwinContext.Response.Headers.Set("Cache-Control", MaxAgeString)
            };
            app.UseStaticFiles(staticOptions);
        }

        public static void UseWindowsAuthentication(this IAppBuilder app)
        {
            var listener = (HttpListener)app.Properties["System.Net.HttpListener"];
            listener.AuthenticationSchemeSelectorDelegate = AuthenticationSchemeSelectorDelegate;
        }

        private static AuthenticationSchemes AuthenticationSchemeSelectorDelegate(HttpListenerRequest httpRequest)
        {
            var originalString = httpRequest.Url.OriginalString;

            for (int index = 0; index < AllowedToAnonymous.Length; index++)
            {
                if (originalString.EndsWith(AllowedToAnonymous[index], StringComparison.Ordinal))
                {
                    return AuthenticationSchemes.Anonymous;
                }
            }

            return AuthenticationSchemes.Ntlm;
        }
    }
}