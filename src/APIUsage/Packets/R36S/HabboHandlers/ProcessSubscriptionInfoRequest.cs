using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public static partial class PacketHandlers
    {
        private static void ProcessSubscriptionInfoRequest(Habbo sender, IncomingMessage message)
        {
            string subscriptionName = (message as ClassicIncomingMessage).PopPrefixedString();
            SubscriptionData data = sender.Subscriptions[subscriptionName];

            new MSubscriptionInfo
                {
                    SubscriptionName = subscriptionName,
                    IsActive = data.IsActive,
                    CurrentDay = data.Expired.Days % 31,
                    ElapsedMonths = data.Expired.Days / 31,
                    PrepaidMonths = data.Remaining.Days / 31
            }.Send(sender);
        }
    }
}
