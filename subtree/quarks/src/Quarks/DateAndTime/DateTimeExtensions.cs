using System;

namespace Codestellation.Quarks.DateAndTime
{
    internal static class DateTimeExtensions
    {
        public static DateTime DiscardMilliseconds(this DateTime time)
        {
            return time.AddMilliseconds(-time.Millisecond);
        }      
    }
}