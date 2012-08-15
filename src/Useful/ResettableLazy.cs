using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bluedot.HabboServer.Useful
{
    public struct ResettableLazy<T>
    {
        public delegate T InstanceGenerator();
        private InstanceGenerator _instanceGenerator;
        private bool _isSet;
        private T _internalValue;
        private object _syncer;

        #region Method: ResettableLazy (Constructor)
        public ResettableLazy(InstanceGenerator instanceGenerator)
        {
            _instanceGenerator = instanceGenerator;
            _isSet = false;
            _internalValue = default(T);
            _syncer = new object();
        }
        #endregion

        public static implicit operator T(ResettableLazy<T> resettable)
        {
            return resettable.Value;
        }
        public T Value
        {
            get
            {
                lock (_syncer)
                {
                    if (!_isSet)
                    {
                        // TODO: Finish
                        _internalValue = _instanceGenerator();
                        _isSet = true;
                    }
                    return _internalValue;
                }
            }
        }
        public void Refresh()
        {
            lock (_syncer)
            {
                _isSet = false;
            }
        }
    }
}