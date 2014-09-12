using System;

namespace Codestellation.Quarks.Native
{
    [AttributeUsage(AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
    internal class UnmanagedProcedureAttribute : Attribute
    {
        public readonly string Name;

        public UnmanagedProcedureAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name should be neither null nor empty string", "name");
            }
            Name = name;
        }
    }
}