using System;

namespace Bluedot.HabboServer.Useful
{

    public class WeakCache<TKey, TValue> where TValue : class
    {
        #region Fields
        #region Field: _instanceGenerator
        private readonly Func<TKey, TValue> _instanceGenerator;
        #endregion
        #region Field: _cache
        /// <summary>
        ///   Stores the cached instances.
        /// </summary>
        //private readonly Dictionary<TKey, WeakReference<TValue>> _cache = new Dictionary<TKey, WeakReference<TValue>>();
        private readonly WeakDictionary<TKey, TValue> _cache = new WeakDictionary<TKey, TValue>();

        public WeakCache(Func<TKey, TValue> instanceGenerator)
        {
            _instanceGenerator = instanceGenerator;
        }

        #endregion
        #endregion

        #region Indexers
        #region Indexer: TKey
        public TValue this[TKey index]
        {
            get
            {
                TValue instance;
                lock (this)
                {
                    // Is this Habbo already cached and has it not yet been collected and removed from memory?
                    if (!_cache.TryGetValue(index, out instance))
                    {
                        // Create a new instance using the implemented ConstructInstance method.
                        instance = _instanceGenerator(index);

                        // And cache it.
                        _cache.Add(index, instance);
                    }
                }

                // Return the newly cached instance.
                return instance;
            }
            set
            {
                lock (this)
                {
                    if (_cache.ContainsKey(index))
                        return;
                    _cache.Add(index, value);
                }
            }
        }
        #endregion
        #endregion
    }
}