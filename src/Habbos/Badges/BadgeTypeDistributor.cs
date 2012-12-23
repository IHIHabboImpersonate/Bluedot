using System;
using System.Collections.Generic;

using Bluedot.HabboServer.Database.Actions;
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
        
        #region Method: GetBadgeCollectionFromHabbo
        public BadgeCollection GetBadgeCollectionFromHabbo(Habbo habbo)
        {
            IDictionary<int, int> badges = BadgeActions.GetBadgeDataFromHabboId(habbo.Id);

            BadgeCollection badgeCollection = new BadgeCollection(badges.Count);

            foreach (KeyValuePair<int, int> badge in badges)
            {
                badgeCollection.AddBadge(this[badge.Key]);
                if (badge.Value != (int)BadgeSlot.NoSlot)
                    badgeCollection.SetBadgeSlot(this[badge.Key], (BadgeSlot)badge.Value);
            }
            return badgeCollection;
        }
        #endregion

        #region Method: CacheInstanceGenerator
        private BadgeType CacheInstanceGenerator(int id)
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
        private BadgeType CacheInstanceGenerator(string code)
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
