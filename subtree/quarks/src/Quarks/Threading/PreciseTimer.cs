using System;
using System.Threading;
using Codestellation.Quarks.DateAndTime;

namespace Codestellation.Quarks.Threading
{
    internal class PreciseTimer : IDisposable
    {
        private static readonly TimeSpan MaxTime = TimeSpan.FromMinutes(1);

        private readonly Timer _internalTimer;
        private DateTime _fireAt;
        private readonly TimerCallback _callback;
        private readonly object _state;
        private bool _shouldFireCallback;

        public PreciseTimer(TimerCallback callback, object state)
        {
            _state = state;
            _callback = callback;
            _internalTimer = new Timer(ignore => OnInternalTimerFired(), this, Timeout.Infinite, Timeout.Infinite);
        }

        public void FireAt(DateTime fireAt)
        {
            if (fireAt.Kind == DateTimeKind.Unspecified)
            {
                throw new ArgumentException("DateTimeKind must be Local or Utc, but was Unspecified", "fireAt");
            }

            _fireAt = fireAt.ToUniversalTime();
            _shouldFireCallback = false;
            SetupTimer(false);
        }

        private void SetupTimer(bool isTimerCallback)
        {
            var fireAfter = _fireAt - Clock.UtcNow;

            if (fireAfter <= TimeSpan.Zero) 
            {
                if (isTimerCallback)
                {
                    _callback(_state);
                }
                else
                {
                    ThreadPool.UnsafeQueueUserWorkItem(state => _callback(state), _state);
                }
            }
            else if (fireAfter > MaxTime)
            {
                _internalTimer.Change(MaxTime, TimeSpan.Zero);
            }
            else
            {
                _shouldFireCallback = true;
                _internalTimer.Change(fireAfter, TimeSpan.Zero);
            }
        }

        private void OnInternalTimerFired()
        {
            if (_shouldFireCallback)
            {
                _callback(_state);
            }
            else
            {
                SetupTimer(true);
            }
        }

        public void Dispose()
        {
            _internalTimer.Dispose();
        }
    }
}