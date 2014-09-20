using System;

namespace Codestellation.Galaxy.Infrastructure.Emisstar
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class SynchronizedAttribute : Attribute
    {
         
    }
}