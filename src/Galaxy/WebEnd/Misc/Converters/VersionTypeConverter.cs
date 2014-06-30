using System;
using Nancy.ModelBinding;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Misc.Converters
{
    public class VersionTypeConverter : ITypeConverter
    {
        public bool CanConvertTo(Type destinationType, BindingContext context)
        {
            return destinationType == typeof(Version);
        }

        public object Convert(string input, Type destinationType, BindingContext context)
        {
            return new Version(input);
        }
    }
}