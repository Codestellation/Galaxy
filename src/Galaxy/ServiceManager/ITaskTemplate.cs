using System;
using System.Collections.Generic;

namespace Codestellation.Galaxy.ServiceManager
{
    public interface ITaskTemplate
    {
        string Name { get; }

        IReadOnlyCollection<Type> Operations { get; }
    }
}