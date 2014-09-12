using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Codestellation.Quarks.StringUtils
{
    internal class TemplateExpander
    {
          private delegate string Renderer(object arg);
        
        public readonly string Body;
        private Dictionary<Type, Renderer> _renderers;
        private readonly Tuple<bool, string>[] _bodyTemplate;
        private readonly bool _noProperties;

        public TemplateExpander(string body)
        {
            Body = body;
            
            _renderers = new Dictionary<Type, Renderer>();

            string buffer = string.Empty;
            bool property = false;
            
            var bodyTemplate = new List<Tuple<bool, string>>();


            for (int charIndex = 0; charIndex < body.Length; charIndex++)
            {
                var symbol = body[charIndex];
            
                if (symbol == '{')
                {
                    if (buffer != string.Empty)
                    {
                        bodyTemplate.Add(Tuple.Create(property, buffer));
                    }

                    buffer = string.Empty;
                    property = true;
                    continue;
                }

                if (symbol == '}')
                {
                    if (buffer != string.Empty)
                    {
                        bodyTemplate.Add(Tuple.Create(property, buffer));
                    }

                    buffer = string.Empty;
                    property = false;
                    continue;
                }
                
                buffer += symbol;

                if (charIndex == body.Length - 1)
                {
                    bodyTemplate.Add(Tuple.Create(property, buffer));
                }
            }

            _bodyTemplate = bodyTemplate.ToArray();
            _noProperties = !bodyTemplate.Any(x => x.Item1);
        }

        public string Render(object model)
        {
            if (_noProperties)
            {
                return Body;
            }
            Renderer renderer;
            if (!_renderers.TryGetValue(model.GetType(), out renderer))
            {
                renderer = BuildAndCacheRenderer(model.GetType());
            }

            return renderer(model);
        }

        private Renderer BuildAndCacheRenderer(Type getType)
        {
            Dictionary<Type, Renderer> afterCas;
            Dictionary<Type, Renderer> beforeCas;
            Renderer value = null;
            do
            {
                beforeCas = _renderers;
                Thread.MemoryBarrier();
                
                Renderer result = null;
                if (beforeCas.TryGetValue(getType, out result))
                {
                    return result;
                }

                if (value == null)
                {
                    value = BuildRenderer(getType);
                }
                var newDictionary = new Dictionary<Type, Renderer>(beforeCas, beforeCas.Comparer) { { getType, value } };

                afterCas = Interlocked.CompareExchange(ref _renderers, newDictionary, beforeCas);
            } while (beforeCas != afterCas);

            return value;
        }

        private Renderer BuildRenderer(Type type)
        {
            var arguments = new Expression[_bodyTemplate.Length];

            var parameter = Expression.Parameter(typeof (object), "input");
            var castedParameter = Expression.Convert(parameter, type);
            
            for (int i = 0; i < _bodyTemplate.Length; i++)
            {
                var template = _bodyTemplate[i];
                var isPropertyOrField = template.Item1;

                if (isPropertyOrField)
                {
                    var propertyName = template.Item2;
                    var property = Expression.PropertyOrField(castedParameter, propertyName);
                    var toStringMethod = Expression.Call(property, "ToString", null, null);
                    arguments[i] = toStringMethod;
                }
                else
                {
                    var constantString = template.Item2;
                    arguments[i] = Expression.Constant(constantString, typeof (string));
                }
            }

            //No concat, just make string of property.
            if (arguments.Length == 1)
            {
                return Expression.Lambda<Renderer>(arguments[0], parameter).Compile();
            }
            
            var concatInfo = SelectConcatMethod(arguments.Length);

            var concat = Expression.Call(concatInfo, arguments);
            var lambda = Expression.Lambda<Renderer>(concat, parameter);
            return lambda.Compile();
        }

        private MethodInfo SelectConcatMethod(int parameterCount)
        {
            if (parameterCount <= 0)
            {
                throw new InvalidOperationException();
            }
            
            var concatMethods = typeof (string).GetMethods().Where(x => x.Name == "Concat");
            
            if (parameterCount <= 4)
            {
                return concatMethods
                    .Where(x => x.GetParameters().All(y => y.ParameterType == typeof(string)))
                    .Single(x =>  x.GetParameters().Length == parameterCount);
            }

            return concatMethods.Single(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof (IEnumerable<string>));
        }


        public override string ToString()
        {
            return Body;
        }
    }
}