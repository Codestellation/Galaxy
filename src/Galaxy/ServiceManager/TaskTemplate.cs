using System;
using System.Collections.Generic;

namespace Codestellation.Galaxy.ServiceManager
{
    public class TaskTemplate : ITaskTemplate
    {
        public TaskTemplate(string name, IEnumerable<Type> operations)
        {
            Name = name;
            var temp = new List<Type>(operations);
            temp.TrimExcess();
            Operations = temp;
        }

        public string Name { get; }
        public IReadOnlyCollection<Type> Operations { get; }
    }
}