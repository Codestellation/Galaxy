using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Owin;
using Nancy;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace Codestellation.Galaxy.WebEnd.Misc
{
    public class StaticContentModule
    {
        private readonly AppFunc _next;

        [ThreadStatic] 
        private static byte[] _buffer;

        public StaticContentModule(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            var request = new OwinRequest(env);

            EmbeddedResource resource;
            if (!EmbeddedResourceList.TryGetResource(request.Path.Value, out resource))
            {
                await _next(env);
                return;
            }

            if (_buffer == null)
            {
                _buffer = new byte[40 * 1024];
            }

            var response = new OwinResponse(env);

            var ifModifiedSince = DateTime.MaxValue;

            string[] values;
            if (request.Headers.TryGetValue("If-Modified-Since", out values))
            {
                ParseDateTime(values[0], ref ifModifiedSince);
            }
                
            ////Cache-Control	max-age=604800
            response.Headers["Cache-Control"] = EmbeddedResourceList.MaxAgeString;
                
            var now = DateTime.UtcNow;
            ////Date	Sat, 25 Jan 2014 10:30:21 GMT
            response.Headers["Date"] = now.ToHttpHeaderDate();
                
            ////Expires	Sat, 01 Feb 2014 10:30:21 GMT            
                
            var expires = now.Add(EmbeddedResourceList.MaxAge);
            response.Headers["Expires"] = expires.ToHttpHeaderDate();
                
            ////Last-Modified	Thu, 05 Dec 2013 17:04:39 GMT
            response.Headers["Last-Modified"] =  EmbeddedResourceList.LastModifiedString;

            response.ContentType = resource.ContentType;
            
            if (ifModifiedSince <= EmbeddedResourceList.Modified)
            {
                response.StatusCode = (int)HttpStatusCode.NotModified;
                //    //X-Cache	HIT
                response.Headers["X-Cache"] = "HIT";
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.OK;

                var content = resource.GetContent();
                response.ContentLength = content.Length;
                
                int read;
                while ((read = content.Read(_buffer, 0, _buffer.Length)) != 0)
                {
                    response.Body.Write(_buffer, 0, read);
                }
            }
        }

        private static void ParseDateTime(string value, ref DateTime modifyMe)
        {
            DateTime result;
            // note CultureInfo.InvariantCulture is ignored
            if (DateTime.TryParseExact(value, "R", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                modifyMe = result;
            }
        }
    }
}