using System;
using System.Threading;

namespace Codestellation.Quarks.Threading
{
    internal static class ThreadExtensions
    {
        public static readonly TimeSpan InfiniteTimeout = TimeSpan.FromMilliseconds(-1);

        public static bool SafeJoin(this Thread self, TimeSpan? timeOut = null)
        {
            if (self == null)
            {
                return true;
            }

            if (self.ThreadState == ThreadState.Unstarted)
            {
                return true;
            }
            return self.Join(timeOut ?? InfiniteTimeout);
        } 
    }
}