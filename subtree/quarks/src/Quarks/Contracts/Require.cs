using System;
using System.Diagnostics;

namespace Codestellation.Quarks.Contracts
{
    internal static class Require
    {
        private const string CompileConstant = "QCONTRACT";

        [Conditional(CompileConstant)]
        public static void Utc(DateTime dateTime, string parameterName = null)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return;
            }

            throw new ArgumentException(string.Format("Expected DateTimeKind.Utc but was {0}.", dateTime.Kind), parameterName);
        }

        [Conditional(CompileConstant)]
        public static void Utc(DateTime? dateTime, string parameterName = null)
        {
            if (dateTime.HasValue)
            {
                Require.Utc(dateTime.Value, parameterName);
            }
        }

        [Conditional(CompileConstant)]
        public static void NotNullOrEmpty(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        [Conditional(CompileConstant)]
        public static void NotNull(object value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        [Conditional(CompileConstant)]
        public static void Null(object value, string parameterName)
        {
            if (value != null)
            {
                throw new ArgumentException(parameterName);
            }
        }


        [Conditional(CompileConstant)]
        public static void NotEmpty(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException(parameterName);
            }
        }

        [Conditional(CompileConstant)]
        public static void GreaterThanZero(decimal value, string parameterName)
        {
            if (value <= 0)
            {
                throw new ArgumentException(string.Format("Should be greater than zero, but was '{0}'", value), parameterName);
            }
        }

        [Conditional(CompileConstant)]
        public static void GreaterOrEqualZero(decimal value, string parameterName)
        {
            if (value < 0)
            {
                throw new ArgumentException(string.Format("Should be greater or equal zero, but was '{0}'", value), parameterName);
            }
        }

        [Conditional(CompileConstant)]
        public static void LesserOrEqualZero(decimal value, string parameterName)
        {
            if (value > 0)
            {
                throw new ArgumentException(string.Format("Should be lesser or equal zero, but was '{0}'", value), parameterName);
            }
        }

        [Conditional(CompileConstant)]
        public static void GreaterThanZero(long value, string parameterName)
        {
            if (value <= 0)
            {
                throw new ArgumentException(string.Format("Should be greater than zero, but was '{0}'", value), parameterName);
            }
        }

        [Conditional(CompileConstant)]
        public static void IsTrue(bool value, string parameterName)
        {
            if (!value)
            {
                throw new ArgumentException("Should be true, but was false", parameterName);
            }
        }

        [Conditional(CompileConstant)]
        public static void IsFalse(bool value, string parameterName)
        {
            if (value)
            {
                throw new ArgumentException("Should be false, but was true", parameterName);
            }
        }

        [Conditional(CompileConstant)]
        public static void Same(object left, string leftParameterName, object right, string rightParameterName)
        {
            if (Equals(left, right) == false)
            {
                var message = string.Format("'{0}' should be same as '{1}', but was '{2}' and '{3}'", leftParameterName, rightParameterName, left, right);
                throw new ArgumentException(message);
            }
        }
    }
}
