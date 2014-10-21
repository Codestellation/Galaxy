using System;
using System.Linq;
using System.Linq.Expressions;

namespace Codestellation.Quarks.Enumerations
{
    internal class EnumIndexer<TEnum, TValue>
    {
        private static readonly int _arraySize;
        private static readonly Func<TEnum, int> ConvertToInt;
        private TValue[] _valueArray;

        static EnumIndexer()
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new InvalidOperationException("Type parameter TEnum must be an Enum");
            }

            var maxValue = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(x => Convert.ToInt32(x)).Max();

            _arraySize = maxValue + 1;

            var enumValue = Expression.Parameter(typeof(TEnum));
            var castToInt = Expression.Convert(enumValue, typeof(int));
            ConvertToInt = Expression.Lambda<Func<TEnum, int>>(castToInt, enumValue).Compile();
        }

        public EnumIndexer()
        {
            InitializeArray();
        }

        private void InitializeArray()
        {

            _valueArray = new TValue[_arraySize];
        }

        public TValue this[TEnum index]
        {
            get
            {
                int intIndex = ConvertToInt(index);
                return _valueArray[intIndex];
            }
            set
            {
                int intIndex = ConvertToInt(index);
                _valueArray[intIndex] = value;
            }
        }

        public TValue[] GetValues()
        {
            return _valueArray;
        }
    }
}