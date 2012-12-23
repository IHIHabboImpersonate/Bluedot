using System.Collections;
using System.Collections.Generic;

namespace Bluedot.HabboServer.Useful
{
    /// <summary>
    /// A dictionary with lazy index loading.
    /// </summary>
    public class LazyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public delegate bool ValueFactory(TKey key, out TValue value);

        private readonly Dictionary<TKey, TValue> _dictionary;

        private readonly ValueFactory _valueFactory;

        #region Method: LazyDictionary (Constructor)
        public LazyDictionary(ValueFactory valueFactory)
        {
            _dictionary = new Dictionary<TKey, TValue>();
            _valueFactory = valueFactory;
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

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_dictionary.ContainsKey(key))
            {
                value = _dictionary[key];
                return true;
            }

            if (!_valueFactory(key, out value))
                return false;

            Add(key, value);
            return true;
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if(!TryGetValue(key, out value))
                    return value;

                throw new KeyNotFoundException();
            }
            set
            {
                _dictionary[key] = value;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return _dictionary.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return _dictionary.Values;
            }
        }


        #region Implementation of IEnumerable

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<KeyValuePair<TKey,TValue>>

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            (_dictionary as ICollection<KeyValuePair<TKey, TValue>>).Add(item);
        }


        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return (_dictionary as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            (_dictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return (_dictionary as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
        }

        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}