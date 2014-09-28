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
        public static string AsString<TEnum>(this TEnum? value) where TEnum : struct
        {
            return EnumUtil<TEnum>.AsString(value);
        }

        public static string AsUpperString<TEnum>(this TEnum value) where TEnum : struct
        {
            return EnumUtil<TEnum>.AsUpperString(value);
        }

        public static string AsUpperString<TEnum>(this TEnum? value) where TEnum : struct
        {
            return EnumUtil<TEnum>.AsUpperString(value);
        }

        public static string AsLowerString<TEnum>(this TEnum value) where TEnum : struct
        {
            return EnumUtil<TEnum>.AsLowerString(value);
        }

        public static string AsLowerString<TEnum>(this TEnum? value) where TEnum : struct
        {
            return EnumUtil<TEnum>.AsLowerString(value);
        }
    }

    internal static class EnumUtil<TEnum> where TEnum : struct
    {
        private static readonly EnumIndexer<TEnum, string> ToStringCache;
        private static readonly EnumIndexer<TEnum, string> ToUpperStringCache;
        private static readonly EnumIndexer<TEnum, string> ToLowerStringCache;

        static EnumUtil()
        {
            ToStringCache = new EnumIndexer<TEnum, string>();
            ToUpperStringCache = new EnumIndexer<TEnum, string>();
            ToLowerStringCache = new EnumIndexer<TEnum, string>();

            foreach (var value in Enum.GetValues(typeof(TEnum)).Cast<TEnum>())
            {
                var valueAsString = value.ToString();
                ToStringCache[value] = valueAsString;
                ToUpperStringCache[value] = valueAsString.ToUpperInvariant();
                ToLowerStringCache[value] = valueAsString.ToLowerInvariant();
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

        public static string AsUpperString(TEnum? value)
        {
            if (value.HasValue)
            {
                return AsString(value.Value);
            }

            return null;
        }

        public static string AsUpperString(TEnum value)
        {
            return ToUpperStringCache[value];
        }

        public static string AsLowerString(TEnum? value)
        {
            if (value.HasValue)
            {
                return AsString(value.Value);
            }

            return null;
        }

        public static string AsLowerString(TEnum value)
        {
            return ToLowerStringCache[value];
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