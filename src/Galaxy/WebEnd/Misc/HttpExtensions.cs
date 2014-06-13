using System;
using System.Globalization;

namespace Codestellation.Galaxy.WebEnd.Misc
{
    public static class HttpExtensions
    {
        public static string ToHttpHeaderDate(this DateTime self)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:ddd, dd MMM yyy hh:mm:ss} GMT", self);
        }
    }
}