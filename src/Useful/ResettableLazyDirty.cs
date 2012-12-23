using System;
using System.Threading;

namespace Bluedot.HabboServer.Useful
{
    /// <summary>
    /// A resettable lazy with dirty tracking.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResettableLazyDirty<T> // What a name...
    {
        private readonly ReaderWriterLockSlim _lock;

        private bool _isDirty;
        private T _dirtyValue;
        
        private readonly Action _valueFactorySetter;
        private readonly Func<T> _valueFactoryReturner;
        private Lazy<T> _lazyValue;

        #region Method: ResettableLazy (Constructor)
        public ResettableLazyDirty(Action valueFactory)
        {
            _lock = new ReaderWriterLockSlim();
            _isDirty = false;

            this._valueFactorySetter = valueFactory;
            _lazyValue = new Lazy<T>(GetValue, LazyThreadSafetyMode.ExecutionAndPublication);
        }
        public ResettableLazyDirty(Func<T> valueFactory)
        {
            _lock = new ReaderWriterLockSlim();
            _isDirty = false;

            this._valueFactoryReturner = valueFactory;
            _lazyValue = new Lazy<T>(GetValue, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private T GetValue()
        {
            if(this._valueFactoryReturner != null)
                return this._valueFactoryReturner();

            this._valueFactorySetter();
            _isDirty = false;
            return _dirtyValue;
        }

        #endregion

        public bool IsValueCreated
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _isDirty || _lazyValue.IsValueCreated;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public bool IsDirty
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _isDirty;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public T Value
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    if(_isDirty)
                        return _dirtyValue;
                    return _lazyValue.Value;
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
                    _dirtyValue = value;
                    _isDirty = true;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        public void Reset()
        {
            _lock.EnterWriteLock();
            try
            {
                _lazyValue = new Lazy<T>(GetValue);
                _isDirty = false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}