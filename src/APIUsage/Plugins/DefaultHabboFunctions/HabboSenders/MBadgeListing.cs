using System.Collections.Generic;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultHabboFunctions
{
    public class MBadgeListing : OutgoingMessage
    {
        #region Property: AllBadges
        /// <summary>
        /// 
        /// </summary>
        public ICollection<string> AllBadges
        {
            get;
            set;
        }
        #endregion
        #region Property: BadgeSlots
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<BadgeSlot, string> BadgeSlots
        {
            get;
            set;
        }
        #endregion

        public override OutgoingMessage Send(IMessageable target)
        {
            if (InternalOutgoingMessage.Id == 0)
            {
                InternalOutgoingMessage.Initialize(229)
                    .AppendInt32(AllBadges.Count);
                foreach (string badge in AllBadges)
                {
                    InternalOutgoingMessage
                        .AppendString(badge);
                }
                foreach (KeyValuePair<BadgeSlot, string> slotBadge in BadgeSlots)
                {
                    InternalOutgoingMessage
                        .AppendInt32((int) slotBadge.Key)
                        .AppendString(slotBadge.Value);
                }
            }

            target.SendMessage(InternalOutgoingMessage);
            return this;
        }
    }

    public enum BadgeSlot
    {
        NoSlot,
        Slot1,
        Slot2,
        Slot3,
        Slot4,
        Slot5
    }
}