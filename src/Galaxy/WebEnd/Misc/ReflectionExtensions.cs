using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace Codestellation.Galaxy.WebEnd.Misc
{
    public static class ReflectionExtensions
    {
        public static string GetMemberName<TModel, TProperty>(this Expression<Func<TModel, TProperty>> property)
        {
            var member = property.GetMember();
            var memberName = member.Name;
            return memberName;
        }

        public static DisplayAttribute GetDisplay(this MemberInfo property)
        {
            var result = property.GetCustomAttributes(typeof(DisplayAttribute), true);
            if (result.Length != 0)
            {
                return (DisplayAttribute)result[0];
            }

            var errorMessage = string.Format("Please mark property {0}.{1} with {2}.", property.DeclaringType, property.Name, typeof(DisplayAttribute));
            throw new InvalidOperationException(errorMessage);
        }

        public static MemberInfo GetMember<TModel, TProperty>(this Expression<Func<TModel, TProperty>> property)
        {
            var memberExpression = (MemberExpression)property.Body;
            return memberExpression.Member;
        }

        public static string ToMemberPath<TDocument, TProperty>(this Expression<Func<TDocument, TProperty>> property)
        {
            var expression = property.Body;

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
            {
                switch (unaryExpression.NodeType)
                {
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        expression = unaryExpression.Operand;
                        break;
                }

            }
            var me = expression as MemberExpression;

            if (me == null)
                throw new InvalidOperationException("No idea how to convert " + property.Body.NodeType + ", " + property.Body + " to a member expression");

            var parts = new List<string>();
            while (me != null)
            {
                parts.Insert(0, me.Member.Name);
                me = me.Expression as MemberExpression;
            }
            return String.Join(".", parts.ToArray());
        } 
    }
}