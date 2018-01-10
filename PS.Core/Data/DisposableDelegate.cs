using System;

namespace PS.Data
{
    public class DisposableDelegate : IDisposable
    {
        private readonly Action _action;

        #region Constructors

        public DisposableDelegate(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _action = action;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _action();
        }

        #endregion
    }
}