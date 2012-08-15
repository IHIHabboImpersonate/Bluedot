using System;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.Habbos
{
    class BadgeTypeDistributor
    {
        #region Fields
        #region Field: _idCache
        private readonly WeakCache<int, BadgeType> _idCache;
        #endregion
        #region Field: _codeCache
        private readonly WeakCache<string, BadgeType> _codeCache;
        #endregion
        #endregion

        #region Indexers
        #region Indexer: int
        public BadgeType this[int id]
        {
            get
            {
                BadgeType result = _idCache[id];
                _codeCache[result.Code] = result;
                return result;
            }
        }
        #endregion
        #region Indexer: string
        public BadgeType this[string code]
        {
            get
            {
                BadgeType result = _codeCache[code];
                _idCache[result.Id] = result;
                return result;
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Method: BadgeTypeDistributor (Constructor)
        public BadgeTypeDistributor()
        {
            _idCache = new WeakCache<int, BadgeType>(CacheInstanceGenerator);
            _codeCache = new WeakCache<string, BadgeType>(CacheInstanceGenerator);
        }
        #endregion

        #region Method: CleanUp
        /// <summary>
        ///   Remove any collected Habbos from the cache.
        /// </summary>
        private void CleanUp()
        {
            // TODO: Look into calling this with http://msdn.microsoft.com/en-us/library/system.gc.registerforfullgcnotification.aspx
            _idCache.CleanUp();
            _codeCache.CleanUp();
        }
        #endregion

        #region Method: CacheInstanceGenerator
        public BadgeType CacheInstanceGenerator(int id)
        {
            try
            {
                return new BadgeType(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
        public BadgeType CacheInstanceGenerator(string code)
        {
            try
            {
                return new BadgeType(code);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
        #endregion
        #endregion
    }
}
