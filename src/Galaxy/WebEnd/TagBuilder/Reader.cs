using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Codestellation.Galaxy.WebEnd.Misc;

namespace Codestellation.Galaxy.WebEnd.TagBuilder
{
    public static class Reader
    {
        private static Dictionary<MemberInfo, object> _expressionCache = new Dictionary<MemberInfo, object>();

        public static TProperty Read<TModel, TProperty>(TModel model, Expression<Func<TModel, TProperty>> property)
        {
            var member = property.GetMember();
            object expression;
            if (!_expressionCache.TryGetValue(member, out expression))
            {
                expression = property.Compile();
                _expressionCache[member] = expression;
            }

            var reader = (Func<TModel, TProperty>) expression;

            return reader(model);
        }
    }
}