using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Codestellation.Galaxy.WebEnd.Misc
{
    public class InputProperties
    {
        public readonly string Path;
        public readonly string Name;
        public readonly string Placeholder;

        private static readonly ConcurrentDictionary<Expression, InputProperties> InputDataCache;

        static InputProperties()
        {
            InputDataCache = new ConcurrentDictionary<Expression, InputProperties>();    
        }

        private InputProperties(string path, string name, string placeholder)
        {
            Path = path;
            Name = name;
            Placeholder = placeholder;
        }

        public static InputProperties Get<TModel, TProperty>(Expression<Func<TModel, TProperty>> property)
        {
            return InputDataCache.GetOrAdd(property,x => BuildProperties(property));
        }

        private static InputProperties BuildProperties<TModel, TProperty>(Expression<Func<TModel, TProperty>> property)
        {
            
            var member = property.GetMember();
            var path = property.ToMemberPath();

            DisplayAttribute display;
            string placeholder = string.Empty;
            string name = member.Name;

            if (member.TryGetDisplay(out display))
            {
                placeholder = display.Prompt;
                name = display.Name;
            }   

            return new InputProperties(path, name, placeholder);

        }
    }
}