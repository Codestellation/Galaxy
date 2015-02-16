using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Codestellation.Quarks.Components
{
    internal struct Optional<TValue>
    {
        private TValue _option;

        public bool HasValue { get; private set; }

        public TValue Value
        {
            get
            {
                if (HasValue == false)
                {
                    throw new InvalidOperationException("Value is empty.");
                }
                return _option;
            }
            set
            {
                HasValue = true;
                _option = value;
            }
        }

        public static explicit operator Optional<TValue>(TValue value)
        {
            return new Optional<TValue> {Value = value};
        }
    }

    internal static class Optional
    {
        private static readonly ConcurrentDictionary<Tuple<Type, string>, Func<object, bool>> ReadersCache = 
            new ConcurrentDictionary<Tuple<Type, string>, Func<object, bool>>(); 
        private static readonly ConcurrentDictionary<Type,  Tuple<Func<object, object>, Func<object, object>>>  ValueReadersCache = 
            new ConcurrentDictionary<Type, Tuple<Func<object, object>, Func<object, object>>>();

        public static Optional<TValue> ToOptional<TValue>(this TValue self)
        {
            return new Optional<TValue> { Value = self };
        }

        public static bool ReadHasValue(object value, string propertyName)
        {
            var key = Tuple.Create(value.GetType(), propertyName);
            var reader = ReadersCache.GetOrAdd(key, BuildReader);
            return reader(value);
        }

        public static object GetValue(object value)
        {
            var reader = ValueReadersCache.GetOrAdd(value.GetType(), BuildReaderWriter);
            return reader.Item1(value);
        }

        public static object From(Type closedOptionalType, object value)
        {
            var writer = ValueReadersCache.GetOrAdd(closedOptionalType, BuildReaderWriter).Item2;
            return writer(value);
        }

        private static Tuple<Func<object, object>, Func<object, object>> BuildReaderWriter(Type arg)
        {
            var reader = BuildValueReader(arg);
            var writer = BuildValueWriter(arg);
            return Tuple.Create(reader, writer);
        }

        private static Func<object, object> BuildValueWriter(Type closedOptionalType)
        {
            
            var valueType = closedOptionalType.GetGenericArguments()[0];
            var valueParameter = Expression.Parameter(typeof(object));
            var castedValue = Expression.Convert(valueParameter, valueType);

            var method = closedOptionalType.GetMethod("op_Explicit", new[] { valueType });

            var optional = Expression.Call(method, castedValue);
            var optionalAsObject = Expression.Convert(optional, typeof (object));

            return Expression.Lambda<Func<object, object>>(optionalAsObject, valueParameter).Compile();
        }

        private static Func<object, object> BuildValueReader(Type arg)
        {
            var valueParameter = Expression.Parameter(typeof (object));
            var castedValue = Expression.Convert(valueParameter, arg);
            var valueProperty = Expression.PropertyOrField(castedValue, "Value");
            var valueAsObject = Expression.Convert(valueProperty, typeof (object));

            var buildValueReader = Expression.Lambda<Func<object, object>>(valueAsObject, valueParameter).Compile();
            return buildValueReader;
        }

        private static Func<object, bool> BuildReader(Tuple<Type, string> arg)
        {
            var valueParameter = Expression.Parameter(typeof (object));
            var castedValue = Expression.Convert(valueParameter, arg.Item1);

            var optionalPropertyValue = Expression.PropertyOrField(castedValue, arg.Item2);

            var hasValue = Expression.PropertyOrField(optionalPropertyValue, "HasValue");

            return Expression.Lambda<Func<object, bool>>(hasValue, valueParameter).Compile();
        }
    }
}
