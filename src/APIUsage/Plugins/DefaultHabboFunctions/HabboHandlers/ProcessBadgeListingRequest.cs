using System.Collections.Generic;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultHabboFunctions
{
    internal static partial class PacketHandlers
    {
        internal static void ProcessBadgeListingRequest(Habbo sender, IncomingMessage message)
        {
            // TODO: Send badges to client
            /*List<string> allBadges = new List<string>(sender.Badges.Count);
            Dictionary<BadgeSlot, string> badgeSlots = new Dictionary<BadgeSlot, string>(5);

            foreach (KeyValuePair<string, BadgeSlot> badge in sender.Badges)
            {
                allBadges.Add(badge.Key);

                if(badge.Value != BadgeSlot.NoSlot)
                    badgeSlots.Add(badge.Value, badge.Key);
            }

            new MBadgeListing
                {
                    AllBadges = allBadges,
                    BadgeSlots = badgeSlots
                }.Send(sender);*/
        }
    }
}
