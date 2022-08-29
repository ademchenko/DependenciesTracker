using System;

namespace DependenciesTracking
{
    internal sealed class Disposable : IDisposable
    {
        private readonly Action _disposeAction;
        private bool _disposed;

        public Disposable(Action disposeAction)
        {
            if (disposeAction == null)
                throw new ArgumentNullException("disposeAction");
            
            _disposeAction = disposeAction;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposeAction();
                _disposed = true;
            }
        }
    }
}
