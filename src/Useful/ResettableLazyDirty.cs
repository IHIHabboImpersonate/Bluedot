using System;
using System.Threading;

namespace IHI.Server.Useful
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
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _isDirty = false;

            _valueFactorySetter = valueFactory;
            _lazyValue = new Lazy<T>(GetFactoryValue, LazyThreadSafetyMode.ExecutionAndPublication);
        }
        public ResettableLazyDirty(Func<T> valueFactory)
        {
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _isDirty = false;

            _valueFactoryReturner = valueFactory;
            _lazyValue = new Lazy<T>(GetFactoryValue, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private T GetFactoryValue()
        {
            if(_valueFactoryReturner != null)
                return _valueFactoryReturner();

            _valueFactorySetter();

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
                if (_isDirty)
                {
                    _lock.EnterReadLock();
                    try
                    {
                        if (_isDirty)
                            return _dirtyValue;
                    }
                    finally
                    {
                        _lock.ExitReadLock();
                    }
                }

                if (_lazyValue.IsValueCreated)
                {
                    _lock.EnterReadLock();
                    try
                    {
                        if (_lazyValue.IsValueCreated)
                            return _lazyValue.Value;
                    }
                    finally
                    {
                        _lock.ExitReadLock();
                    }
                }

                _lock.EnterWriteLock();
                try
                {
                    if (_isDirty)
                        return _dirtyValue;
                    return _lazyValue.Value;
                }
                finally
                {
                    _lock.ExitWriteLock();
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
                _lazyValue = new Lazy<T>(GetFactoryValue);
                _isDirty = false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}