using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Codestellation.Quarks.Exceptions
{
    internal static class ExceptionExtensions
    {
        private static readonly Action<Exception> PreserveStackTrace;

        static ExceptionExtensions()
        {
            var parameter = Expression.Parameter(typeof(Exception));
            var methodInfo = typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);
            var method = Expression.Call(parameter, methodInfo);
            var lambda = Expression.Lambda<Action<Exception>>(method, parameter);

            PreserveStackTrace = lambda.Compile();
        }

        public static Exception WithPreservedStackTrace(this Exception self)
        {
            PreserveStackTrace(self);
            return self;
        }
    }
}