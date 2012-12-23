using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace Bluedot.HabboServer.Useful
{
    /// <summary>
    /// A dictionary with lazy index loading.
    /// </summary>
    public class HalfLazyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        #region Fields
        #region Field: _dictionary
        private readonly Dictionary<TKey, TValue> _dictionary;
        #endregion
        #region Field: _lazyKeys
        private readonly HashSet<TKey> _lazyKeys;
        #endregion

        #region Field: _valueFactory
        private readonly Func<TKey, TValue> _valueFactory;
        #endregion
        #endregion

        #region Properties
        #region Property: Keys
        public ICollection<TKey> Keys
        {
            get
            {
                return _lazyKeys.Union(_dictionary.Keys).ToArray();
            }
        }
        #endregion
        #region Properties: Values
        public ICollection<TValue> Values
        {
            get
            {
                LoadLazyKeys();
                return _dictionary.Values;
            }
        }
        #endregion

        #region Property: Count
        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }
        #endregion

        #region Property: ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
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
                if (!TryGetValue(key, out value))
                    return value;

                throw new KeyNotFoundException();
            }
            set
            {
                _dictionary[key] = value;
                _lazyKeys.Remove(key);
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Method: HalfLazyDictionary (Constructor)
        public HalfLazyDictionary(Func<TKey, TValue> valueFactory)
        {
            _dictionary = new Dictionary<TKey, TValue>();
            _lazyKeys = new HashSet<TKey>();
            _valueFactory = valueFactory;
        }
        #endregion

        #region Method: LoadLazyKeys
        private void LoadLazyKeys()
        {
            foreach (TKey lazyKey in _lazyKeys)
            {
                _dictionary.Add(lazyKey, _valueFactory(lazyKey));
            }
            _lazyKeys.Clear();
        }
        #endregion

        #region Method: Add
        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            _lazyKeys.Remove(key);
        }
        #endregion
        #region Method: AddLazy
        public void AddLazy(TKey key)
        {
            if(!_dictionary.ContainsKey(key))
                _lazyKeys.Add(key);
        }
        #endregion

        #region Method: Remove
        public bool Remove(TKey key)
        {
            return _dictionary.Remove(key) || _lazyKeys.Remove(key);
        }

        #endregion
        #region Method: Clear
        public void Clear()
        {
            _dictionary.Clear();
            _lazyKeys.Clear();
        }
        #endregion

        #region Method: ContainsKey
        public bool ContainsKey(TKey key)
        {
            if (_dictionary.ContainsKey(key))
                return true;

            TValue value;
            if (!_valueFactory(key, out value))
                return false;

            Add(key, value);
            return true;
        }
        #endregion
        #region Method: TryGetValue
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_dictionary.ContainsKey(key))
            {
                value = _dictionary[key];
                return true;
            }

            if (_lazyKeys.Contains(key))
            {
                _lazyKeys.Remove(key);
                value = _valueFactory(key);
                _dictionary.Add(key, _valueFactory(key));
                return true;
            }

            value = default(TValue);
            return false;
        }
        #endregion
        
        #region Method: ICollection<KeyValuePair<TKey, TValue>>.Add
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }
        #endregion
        #region Method: ICollection<KeyValuePair<TKey, TValue>>.Remove
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
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
            (_dictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }
        #endregion

        #region Method: GetEnumerator
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            LoadLazyKeys();
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
        #endregion
    }
}