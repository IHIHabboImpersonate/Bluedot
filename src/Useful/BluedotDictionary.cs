using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bluedot.HabboServer.Useful
{
    // TODO: Thread Safety
    // TODO: WeakReferenceBehaviour.RemoveCollected


    /// <summary>
    /// A dictionary with many additional behaviours
    /// </summary>
    public class BluedotDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TValue : class
    {
        #region Fields
        #region Field: _strongDictionary
        private readonly Dictionary<TKey, TValue> _strongDictionary;
        #endregion
        #region Field: _weakDictionary
        private readonly Dictionary<TKey, WeakReference<TValue>> _weakDictionary;
        #endregion
        #region Field: _lazyKeys
        private readonly HashSet<TKey> _lazyKeys;
        private readonly HashSet<TKey> _dirtyKeys;
        #endregion
        #endregion

        #region Properties
        #region Behaviours
        #region Property: LazyLoading
        public LazyLoadingBehaviour LazyLoading
        {
            get;
            private set;
        }
        #endregion
        #region Property: DirtyTracking
        public DirtyTrackingBehaviour DirtyTracking
        {
            get;
            private set;
        }
        #endregion
        #region Property: ReadOnly
        public ReadOnlyBehaviour ReadOnly
        {
            get;
            private set;
        }
        #endregion
        #region Property: WeakReferenceBehaviour
        public WeakReferenceBehaviour WeakReference
        {
            get;
            private set;
        }
        #endregion
        #endregion
        
        #region Property: Keys
        public ICollection<TKey> Keys
        {
            get
            {
                if (WeakReference == null || !WeakReference.Values)
                {
                    if (LazyLoading == null || !LazyLoading.Values)
                        return _strongDictionary.Keys;
                    return _lazyKeys.Union(_strongDictionary.Keys).ToArray();
                }

                if (LazyLoading == null || !LazyLoading.Values)
                    return _weakDictionary.Keys;
                return _lazyKeys.Union(_weakDictionary.Keys).ToArray();
            }
        }

        #endregion
        #region Properties: Values
        public ICollection<TValue> Values
        {
            get
            {
                LoadLazyKeys();

                if (WeakReference == null || !WeakReference.Values)
                    return _strongDictionary.Values;

                ICollection<TValue> values = new List<TValue>(_weakDictionary.Count);
                foreach (KeyValuePair<TKey, TValue> pair in WeakEnumerable())
                {
                    values.Add(pair.Value);
                }
                return values;
            }
        }
        #endregion
        #region Property: Count
        public int Count
        {
            get
            {
                if (WeakReference == null || !WeakReference.Values)
                {
                    if (LazyLoading == null || !LazyLoading.Values)
                        return _strongDictionary.Count;
                    return _strongDictionary.Count + _lazyKeys.Count;
                }

                if (LazyLoading == null || !LazyLoading.Values)
                    return _weakDictionary.Count;
                return _weakDictionary.Count + _lazyKeys.Count;
            }
        }
        #endregion

        #region Property: ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                if (ReadOnly == null)
                    return false;

                if (ReadOnly.Keys || ReadOnly.Values)
                    return true;
                return false;
            }
        }

        #endregion
        #endregion

        #region Indexers
        #region Indexer: TKey
        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (TryGetValue(key, out value))
                    return value;

                throw new KeyNotFoundException();
            }
            set
            {
                if (ReadOnly != null && ReadOnly.Values)
                    return;


                if (WeakReference == null || !WeakReference.Values)
                    _strongDictionary[key] = value;
                else
                    _weakDictionary[key] = new WeakReference<TValue>(value);


                if (LazyLoading != null && LazyLoading.Values)
                    _lazyKeys.Remove(key);

                if (DirtyTracking != null && DirtyTracking.Values)
                    _dirtyKeys.Add(key);

            }
        }
        #endregion
        #endregion

        #region Methods
        #region Method: BluedotDictionary (Constructor)
        public BluedotDictionary(LazyLoadingBehaviour lazyLoading = null, DirtyTrackingBehaviour dirtyTracking = null, ReadOnlyBehaviour readOnly = null, WeakReferenceBehaviour weakReference = null)
        {
            LazyLoading = lazyLoading;
            DirtyTracking = dirtyTracking;
            ReadOnly = readOnly;
            WeakReference = weakReference;

            if (LazyLoading != null && LazyLoading.Values)
                _lazyKeys = new HashSet<TKey>();

            if (DirtyTracking != null && DirtyTracking.Values)
                _dirtyKeys = new HashSet<TKey>();

            if (WeakReference != null && WeakReference.Values)
                _weakDictionary = new Dictionary<TKey, WeakReference<TValue>>();
            else
                _strongDictionary = new Dictionary<TKey, TValue>();
        }
        #endregion

        #region Method: LoadLazyKeys
        private void LoadLazyKeys()
        {
            if (LazyLoading == null || !LazyLoading.Values)
                return;

            if (WeakReference == null || !WeakReference.Values)
            {
                foreach (TKey lazyKey in _lazyKeys)
                {
                    _strongDictionary.Add(lazyKey, LazyLoading.ValueFactory(lazyKey));
                }
            }
            else
            {
                foreach (TKey lazyKey in _lazyKeys)
                {
                    _weakDictionary.Add(lazyKey, new WeakReference<TValue>(LazyLoading.ValueFactory(lazyKey)));
                }
            }
            _lazyKeys.Clear();
        }
        #endregion

        #region Method: Add
        public void Add(TKey key, TValue value)
        {
            if (ReadOnly != null && (ReadOnly.Keys || ReadOnly.Values))
                return;

            if (WeakReference == null || !WeakReference.Values)
                _strongDictionary.Add(key, value);
            else
                _weakDictionary.Add(key, new WeakReference<TValue>(value));

            if (LazyLoading != null && LazyLoading.Values)
                _lazyKeys.Remove(key);
        }
        #endregion
        #region Method: AddLazy
        public void AddLazy(TKey key)
        {
            if (ReadOnly != null && (ReadOnly.Keys || ReadOnly.Values))
                return;

            if (LazyLoading == null || !LazyLoading.Values)
                throw new NotSupportedException("This BluedotDictionary instance does not have the LazyLoading behaviour enabled.");

            if (WeakReference == null || !WeakReference.Values)
            {
                if (!_strongDictionary.ContainsKey(key))
                    _lazyKeys.Add(key);
            }
            else
            {
                if (!_weakDictionary.ContainsKey(key))
                    _lazyKeys.Add(key);
            }
        }
        #endregion

        #region Method: Remove
        public bool Remove(TKey key)
        {
            if (ReadOnly != null && (ReadOnly.Keys || ReadOnly.Values))
                return false;


            if (WeakReference == null || !WeakReference.Values)
            {
                if (LazyLoading == null || !LazyLoading.Values)
                    return _strongDictionary.Remove(key);

                return _strongDictionary.Remove(key) || _lazyKeys.Remove(key);
            }
            if (LazyLoading == null || !LazyLoading.Values)
                return _weakDictionary.Remove(key);

            return _weakDictionary.Remove(key) || _lazyKeys.Remove(key);
        }

        #endregion
        #region Method: Clear
        public void Clear()
        {
            if (ReadOnly != null && (ReadOnly.Keys || ReadOnly.Values))
                return;


            if (WeakReference == null || !WeakReference.Values)
                _strongDictionary.Clear();
            else
                _weakDictionary.Clear();

            if (LazyLoading != null && LazyLoading.Values)
                _lazyKeys.Clear();
        }
        #endregion

        #region Method: ContainsKey
        public bool ContainsKey(TKey key)
        {
            if (WeakReference == null || !WeakReference.Values)
            {
                if (_strongDictionary.ContainsKey(key))
                    return true;
            }
            else
            {
                if (_weakDictionary.ContainsKey(key))
                    return true;
            }


            if (LazyLoading == null || !LazyLoading.Values)
                return false;

            return _lazyKeys.Contains(key);
        }
        #endregion
        #region Method: TryGetValue
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (WeakReference == null || !WeakReference.Values)
            {
                if (_strongDictionary.ContainsKey(key))
                {
                    value = _strongDictionary[key];
                    return true;
                }

                if (LazyLoading != null && LazyLoading.Values)
                {
                    if (_lazyKeys.Contains(key))
                    {
                        _lazyKeys.Remove(key);
                        value = LazyLoading.ValueFactory(key);
                        _strongDictionary.Add(key, value);
                        return true;
                    }
                    if(LazyLoading.Keys)
                    {
                        value = LazyLoading.ValueFactory(key);
                        _strongDictionary.Add(key, value);
                        return true;
                    }
                }
            }
            else
            {
                if (_weakDictionary.ContainsKey(key))
                {
                    if (_weakDictionary[key].TryGetTarget(out value))
                        return true;
                    if (WeakReference.Values)
                        _weakDictionary[key] = new WeakReference<TValue>(value);
                    return true;
                }

                if (LazyLoading != null && LazyLoading.Values)
                {
                    if (_lazyKeys.Contains(key))
                    {
                        _lazyKeys.Remove(key);
                        value = LazyLoading.ValueFactory(key);
                        _weakDictionary.Add(key, new WeakReference<TValue>(value));
                        return true;
                    }
                    if (LazyLoading.Keys)
                    {
                        value = LazyLoading.ValueFactory(key);
                        _weakDictionary.Add(key, new WeakReference<TValue>(value));
                        return true;
                    }
                }
            }

            value = default(TValue);
            return false;
        }

        #endregion
        
        #region Method: ICollection<KeyValuePair<TKey, TValue>>.Add
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {       
            if (ReadOnly != null && (ReadOnly.Keys || ReadOnly.Values))
                return;

            Add(item.Key, item.Value);
        }
        #endregion
        #region Method: ICollection<KeyValuePair<TKey, TValue>>.Remove
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (ReadOnly != null && (ReadOnly.Keys || ReadOnly.Values))
                return false;

            if ((this as ICollection<KeyValuePair<TKey, TValue>>).Contains(item))
                return Remove(item.Key);
            return false;
        }
        #endregion
        #region Method: ICollection<KeyValuePair<TKey, TValue>>.Contains
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            if(TryGetValue(item.Key, out value))
                return value.Equals(item.Value);
            return false;
        }
        #endregion
        #region Method: ICollection<KeyValuePair<TKey, TValue>>.CopyTo
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            LoadLazyKeys();
            if (WeakReference == null || !WeakReference.Values)
                (_strongDictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
            else
            {
                // This isn't the best way of doing this but it saves me lots of time. Programmer time is more valuable than CPU time.
                ICollection<KeyValuePair<TKey, TValue>> tempDictionary = new Dictionary<TKey, TValue>(_weakDictionary.Count);
                foreach (KeyValuePair<TKey, TValue> pair in WeakEnumerable())
                {
                    tempDictionary.Add(pair);
                }

                tempDictionary.CopyTo(array, arrayIndex);
            }
        }
        #endregion

        #region Method: GetEnumerator
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            LoadLazyKeys();
            if (WeakReference == null || !WeakReference.Values)
                return _strongDictionary.GetEnumerator();
            return WeakEnumerable().GetEnumerator();
        }
        private IEnumerable<KeyValuePair<TKey, TValue>> WeakEnumerable()
        {
            foreach (KeyValuePair<TKey, WeakReference<TValue>> weakPair in _weakDictionary)
            {
                TValue tempValue;
                if (weakPair.Value.TryGetTarget(out tempValue))
                    yield return new KeyValuePair<TKey, TValue>(weakPair.Key, tempValue);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
        #endregion

        #region Types
        #region Type: LazyLoadingBehaviour
        public class LazyLoadingBehaviour
        {
            public bool Keys
            {
                get;
                private set;
            }

            public bool Values
            {
                get;
                private set;
            }

            public Func<TKey, TValue> ValueFactory
            {
                get;
                private set;
            }

            public LazyLoadingBehaviour(bool lazyKeys, bool lazyValues, Func<TKey, TValue> lazyValueFactory)
            {
                Keys = lazyKeys;
                Values = lazyValues;
                ValueFactory = lazyValueFactory;
            }
        }
        #endregion
        #region Type: DirtyTrackingBehaviour
        public class DirtyTrackingBehaviour
        {
            public bool Values
            {
                get;
                private set;
            }

            public DirtyTrackingBehaviour(bool dirtyValueTracking)
            {
                Values = dirtyValueTracking;
            }
        }
        #endregion
        #region Type: ReadOnlyBehaviour
        public class ReadOnlyBehaviour
        {
            public bool Values
            {
                get;
                private set;
            }
            public bool Keys
            {
                get;
                private set;
            }

            public ReadOnlyBehaviour(bool readonlyValues, bool readonlyKeys)
            {
                Values = readonlyValues;
                Keys = readonlyKeys;
            }
        }
        #endregion
        #region Type: WeakReferenceBehaviour
        public class WeakReferenceBehaviour
        {
            /// <summary>
            /// Act as cache.
            /// </summary>
            public bool RemoveCollected
            {
                get;
                private set;
            }

            public bool Values
            {
                get;
                private set;
            }

            public WeakReferenceBehaviour(bool weakValues)
            {
                Values = weakValues;
            }
        }
        #endregion
        #endregion
    }
}