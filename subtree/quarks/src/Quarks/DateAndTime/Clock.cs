using System;

namespace Codestellation.Quarks.DateAndTime
{
    internal static class Clock
    {
        private static Func<DateTime> _dateTimeGenerator = () => DateTime.UtcNow;
        
        public static DateTime UtcNow
        {
            get { return _dateTimeGenerator(); }
        }

        /// <summary>
        /// Test purpose only. 
        /// </summary>
        public static void FixClockAt(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                throw new ArgumentException("DateTimeKind must be Local or Utc, but was Unspecified", "dateTime");
            }
            //It's here to make reading of test output more easier
            Console.WriteLine("Clock fixed at {0}", dateTime);

            var fixedDateTime = dateTime.ToUniversalTime();
            _dateTimeGenerator = () => fixedDateTime;
        }

        public static void SetRealTime()
        {
            _dateTimeGenerator = () => DateTime.UtcNow;
        }
    }
}