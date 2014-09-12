using System;
using System.Linq;

namespace Codestellation.Quarks.Enumerations
{
    internal static class EnumUtil
    {
        public static string AsString<TEnum>(this TEnum value) where TEnum : struct
        {
            return EnumUtil<TEnum>.AsString(value);
        }
    }

    internal static class EnumUtil<TEnum> where TEnum : struct
    {
        public static readonly EnumIndexer<TEnum, string> ToStringCache;

        static EnumUtil()
        {
            ToStringCache = new EnumIndexer<TEnum, string>();

            foreach (var value in Enum.GetValues(typeof(TEnum)).Cast<TEnum>())
            {
                ToStringCache[value] = value.ToString();
            }
        }

        public static string AsString(TEnum? value)
        {
            if (value.HasValue)
            {
                return AsString(value.Value);
            }

            return null;
        }

        public static string AsString(TEnum value)
        {
            return ToStringCache[value];
        }

        public static TEnum Parse(string value)
        {
            TEnum result;
            if (Enum.TryParse(value, true, out result))
            {
                return result;
            }

            var message = string.Format("Could not parse value {0} to enum {1}", value, typeof (TEnum));
            throw new FormatException(message);
        }
    }
}