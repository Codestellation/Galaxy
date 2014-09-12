namespace Codestellation.Quarks.StringUtils
{
    internal static class StringExtensions
    {
        public static string FormatWith(this string self, params object[] args)
        {
            return string.Format(self, args);
        }
    }
}