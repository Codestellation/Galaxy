using System;
using System.Collections.Generic;
using Codestellation.Quarks.Collections;

namespace Codestellation.Galaxy.Infrastructure
{
    public class TimeZoneCollection
    {
        static TimeZoneCollection()
        {
            AvailableTimeZones = TimeZoneInfo
                .GetSystemTimeZones()
                .ConvertToArray(tzi => new KeyValuePair<string, string>(tzi.Id, tzi.DisplayName));
        }

        public static KeyValuePair<string, string>[] AvailableTimeZones { get; private set; }
    }
}