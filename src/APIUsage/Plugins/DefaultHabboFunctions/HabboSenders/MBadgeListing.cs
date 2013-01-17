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
        public ICollection<BadgeType> AllBadges
        {
            get;
            set;
        }
        #endregion
        #region Property: BadgeSlots
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<BadgeSlot, BadgeType> BadgeSlots
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
                foreach (BadgeType badge in AllBadges)
                {
                    InternalOutgoingMessage
                        .AppendString(badge.Code);
                }
                foreach (KeyValuePair<BadgeSlot, BadgeType> slotBadge in BadgeSlots)
                {
                    InternalOutgoingMessage
                        .AppendInt32((int) slotBadge.Key)
                        .AppendString(slotBadge.Value.Code);
                }
            }

            target.SendMessage(InternalOutgoingMessage);
            return this;
        }
    }
}