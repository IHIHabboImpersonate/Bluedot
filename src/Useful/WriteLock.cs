using System.Threading;

namespace Bluedot.HabboServer.Useful
{
    /// <typeparam name="T"></typeparam>
    public class WriteLock<T>
    {
        private readonly ReaderWriterLockSlim _lock;
        private T _value;

        #region Method: WriteLock (Constructor)
        public WriteLock()
        {
            _lock = new ReaderWriterLockSlim();
        }
        #endregion

        public T Value
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _value;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _value = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
    }
}