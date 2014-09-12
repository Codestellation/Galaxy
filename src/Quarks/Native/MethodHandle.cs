using Microsoft.Win32.SafeHandles;

namespace Codestellation.Quarks.Native
{
    internal class MethodHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public MethodHandle() : base(false)
        {
        }

        protected override bool ReleaseHandle()
        {
            return false;
        }
    }
}