using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Codestellation.Galaxy.WebEnd.Misc;

namespace Codestellation.Galaxy.WebEnd.TagBuilder
{
    public static class FastPropertyReader
    {
        private static readonly Dictionary<MemberInfo, object> ExpressionCache = new Dictionary<MemberInfo, object>();

        public static TProperty Read<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> property)
        {
            var member = property.GetMember();
            object expression;
            if (!ExpressionCache.TryGetValue(member, out expression))
            {
                expression = property.Compile();
                ExpressionCache[member] = expression;
            }

            var reader = (Func<TModel, TProperty>) expression;

            return reader(model);
        }
    }
}