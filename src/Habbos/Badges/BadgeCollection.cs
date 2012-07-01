using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.Habbos
{
    public class BadgeCollection
    {
        #region Fields
        #region Field: _badges
        private Dictionary<BadgeType, BadgeSlot> _badges;
        #endregion
        #region Field: _slots
        private BadgeType[] _slots;
        #endregion
        #endregion

        #region Property: BadgeDictionary
        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyDictionary<BadgeType, BadgeSlot> BadgeDictionary
        {
            get
            {
                return new ReadOnlyDictionary<BadgeType, BadgeSlot>(_badges);
            }
        }
        #endregion

        #region Indexers
        public BadgeType this[BadgeSlot slot]
        {
            get
            {
                if (slot == BadgeSlot.NoSlot)
                    throw new ArgumentException("Cannot get badge in NoSlot.", "slot");
                lock (this)
                {
                    return _slots[(int) slot - 1];
                }
            }
            set
            {
                if (slot == BadgeSlot.NoSlot)
                    throw new ArgumentException("Cannot set badge in NoSlot.", "slot");

                lock (this)
                {
                    if (_slots == null)
                        _slots = new BadgeType[5];

                    BadgeType oldValue = _slots[(int) slot - 1];

                    if (oldValue != null)
                        _badges[oldValue] = BadgeSlot.NoSlot;

                    if (value != null)
                        _badges[value] = slot;

                    _slots[(int) slot - 1] = value;
                }
            }
        }
        #endregion

        #region Method: BadgeCollection (Constructor)
        public BadgeCollection(IEnumerable<BadgeType> badges)
        {
            _badges = new Dictionary<BadgeType, BadgeSlot>();
            foreach (BadgeType badge in badges)
                _badges.Add(badge, BadgeSlot.NoSlot);
        }
        #endregion
    }
}
