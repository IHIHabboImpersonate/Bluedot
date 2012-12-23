using System.Collections.Generic;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public static partial class PacketHandlers
    {
        private static void ProcessBadgeListingRequest(Habbo sender, IncomingMessage message)
        {
            List<BadgeType> allBadges = new List<BadgeType>(sender.Badges.Count);
            Dictionary<BadgeSlot, BadgeType> badgeSlots = new Dictionary<BadgeSlot, BadgeType>(5);

            foreach (KeyValuePair<BadgeType, BadgeSlot> badge in sender.Badges)
            {
                allBadges.Add(badge.Key);

                if(badge.Value != BadgeSlot.NoSlot)
                    badgeSlots.Add(badge.Value, badge.Key);
            }

            new MBadgeListing
                {
                    AllBadges = allBadges,
                    BadgeSlots = badgeSlots
                }.Send(sender);
        }
    }
}
