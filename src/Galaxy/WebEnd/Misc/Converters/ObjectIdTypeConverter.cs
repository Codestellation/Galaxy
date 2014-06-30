using System;
using Nancy.ModelBinding;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Misc.Converters
{
    public class ObjectIdTypeConverter : ITypeConverter
    {
        public bool CanConvertTo(Type destinationType, BindingContext context)
        {
            return destinationType == typeof(ObjectId);
        }

        public object Convert(string input, Type destinationType, BindingContext context)
        {
            return new ObjectId(input);
        }
    }
}