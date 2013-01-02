using System;

namespace Bluedot.HabboServer.Useful
{

    public class WeakCache<TKey, TValue> where TValue : class
    {
        #region Fields
        #region Field: _cache
        /// <summary>
        ///   Stores the cached instances.
        /// </summary>
        private readonly BluedotDictionary<TKey, TValue> _cache;
        #endregion
        #region Field: _weakCacheWeakReferenceBehaviour
        private readonly BluedotDictionary<TKey, TValue>.WeakReferenceBehaviour _weakCacheWeakReferenceBehaviour;
        #endregion
        #region Field: _weakCacheLazyLoadingBehaviour
        private readonly BluedotDictionary<TKey, TValue>.LazyLoadingBehaviour _weakCacheLazyLoadingBehaviour;
        #endregion
        #endregion

        #region Methods
        #region Method: WeakCache (Constructor)
        public WeakCache(Func<TKey, TValue> instanceGenerator)
        {
            _weakCacheWeakReferenceBehaviour = new BluedotDictionary<TKey, TValue>.WeakReferenceBehaviour(true);
            _weakCacheLazyLoadingBehaviour = new BluedotDictionary<TKey, TValue>.LazyLoadingBehaviour(true, instanceGenerator);

             _cache = new BluedotDictionary<TKey, TValue>(weakReference: _weakCacheWeakReferenceBehaviour, lazyLoading: _weakCacheLazyLoadingBehaviour);
        }
        #endregion
        #endregion

        #region Indexers
        #region Indexer: TKey
        public TValue this[TKey index]
        {
            get
            {
                return _cache[index];
            }
            set
            {
                _cache[index] = value;
            }
        }
        #endregion
        #endregion
    }
}