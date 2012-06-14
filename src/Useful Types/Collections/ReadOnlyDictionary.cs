using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Bluedot.HabboServer.Collections
{
    public sealed class ReadOnlyDictionary<TKey, TValue> : ReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        public ReadOnlyDictionary(IDictionary<TKey, TValue> items)
            : base(items.ToList()) { }

        public TValue this[TKey key]
        {
            get
            {
                var valueQuery = GetQuery(key);
                if (valueQuery.Count() == 0)
                    throw new NullReferenceException("No value found for given key");

                return valueQuery.First().Value;
            }
        }

        public bool ContainsKey(TKey key)
        {
            return (GetQuery(key).Count() > 0);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var toReturn = ContainsKey(key);
            value = toReturn ? this[key] : default(TValue);
            return toReturn;
        }

        private IEnumerable<KeyValuePair<TKey, TValue>> GetQuery(TKey key)
        {
            return (from t in base.Items where t.Key.Equals(key) select t);
        }
    }
}
