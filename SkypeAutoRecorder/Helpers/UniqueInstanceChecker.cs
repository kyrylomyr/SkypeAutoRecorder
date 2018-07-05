using System;
using System.Threading;

namespace SkypeAutoRecorder.Helpers
{
    /// <summary>
    /// Provides a possibility to check that only one application instance is running.
    /// </summary>
    internal class UniqueInstanceChecker
    {
        private readonly Mutex _mutex;
        private readonly bool _owner;

        public UniqueInstanceChecker(string uniqueId)
        {
            _mutex = new Mutex(true, uniqueId, out _owner);
        }

        public bool IsAlreadyRunning()
        {
            return !_mutex.WaitOne(TimeSpan.Zero, true);
        }

        public void Release()
        {
            if (_owner)
                _mutex.ReleaseMutex();
        }
    }
}
