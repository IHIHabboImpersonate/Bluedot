using System;
using System.Collections;
using System.Collections.Generic;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.Habbos
{
    public class BadgeCollection : IEnumerable<KeyValuePair<BadgeType, BadgeSlot>>
    {
        #region Fields
        #region Field: _badges
        private readonly Dictionary<BadgeType, BadgeSlot> _badges;
        #endregion
        #region Field: _slots
        private BadgeType[] _slots;
        #endregion
        #endregion
        
        #region Property: Count
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return _badges.Count;
            }
        }
        #endregion

        #region Indexers

        public BadgeType this[BadgeSlot slot]
        {
            get
            {
                return GetBadgeInSlot(slot);
            }
            set
            {

                SetBadgeInSlot(slot, value);
            }
        }

        #endregion

        #region Method: BadgeCollection (Constructor)
        public BadgeCollection(int capacity = 0)
        {
            _badges = new Dictionary<BadgeType, BadgeSlot>(capacity);
            _slots = new BadgeType[5];
        }
        #endregion

        #region Method: ContainsBadge
        public bool ContainsBadge(BadgeType badge)
        {
            return _badges.ContainsKey(badge);
        }
        #endregion

        #region Method: AddBadge
        public BadgeCollection AddBadge(BadgeType badge)
        {
            lock (this)
            {
                if (!ContainsBadge(badge))
                    _badges.Add(badge, BadgeSlot.NoSlot);
            }
            return this;
        }
        #endregion
        #region Method: RemoveBadge
        public BadgeCollection RemoveBadge(BadgeType badge)
        {
            lock (this)
            {
                if (ContainsBadge(badge))
                    _badges.Remove(badge);
            }
            return this;
        }
        #endregion

        #region Method: GetBadgeSlot
        public BadgeSlot GetBadgeSlot(BadgeType badge)
        {
            lock (this)
            {
                if (!ContainsBadge(badge))
                    throw new KeyNotFoundException("The given BadgeType was not in this BadgeCollection.");
                return _badges[badge];
            }
        }
        #endregion
        #region Method: TryGetBadgeSlot
        public bool TryGetBadgeSlot(BadgeType badge, out BadgeSlot slot)
        {
            lock (this)
            {
                if (!ContainsBadge(badge))
                {
                    slot = BadgeSlot.NoSlot;
                    return false;
                }
                slot = _badges[badge];
                return true;
            }
        }
        #endregion
        #region Method: SetBadgeSlot
        public BadgeCollection SetBadgeSlot(BadgeType badge, BadgeSlot slot)
        {
            lock(this)
            {
                if (!ContainsBadge(badge))
                    throw new KeyNotFoundException("The given BadgeType was not in this BadgeCollection.");
                _badges[badge] = slot;
                _slots[(int)slot - 1] = badge;
            }
            return this;
        }
        #endregion

        #region Method: GetBadgeInSlot
        public BadgeType GetBadgeInSlot(BadgeSlot slot)
        {
            if (slot == BadgeSlot.NoSlot)
                throw new ArgumentException("Cannot get badge in slot NoSlot.", "slot");

            return _slots[(int)slot - 1];
        }
        #endregion
        #region Method: SetBadgeInSlot
        public BadgeCollection SetBadgeInSlot(BadgeSlot slot, BadgeType badge)
        {
            if (slot == BadgeSlot.NoSlot)
                throw new ArgumentException("Cannot get badge in slot NoSlot.", "slot");

            lock(this)
            {
                if (badge != null && !ContainsBadge(badge))
                    throw new KeyNotFoundException("The given BadgeType was not in this BadgeCollection. No changes to the BadgeSlot was made.");

                BadgeType oldBadgeInSlot = _slots[(int)slot - 1];
                
                if(oldBadgeInSlot != null)
                    _badges[oldBadgeInSlot] = BadgeSlot.NoSlot;

                _slots[(int)slot - 1] = badge;

                if (badge == null)
                    return this;
                _badges[badge] = slot;
            }
            return this;
        }
        #endregion

        #region Method: GetEnumerator
        public IEnumerator<KeyValuePair<BadgeType, BadgeSlot>> GetEnumerator()
        {
            return _badges.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
