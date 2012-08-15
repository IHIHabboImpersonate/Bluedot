using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public static partial class PacketHandlers
    {
        private static void ProcessSubscriptionDataRequest(Habbo sender, IncomingMessage message)
        {
            string subscriptionName = (message as ClassicIncomingMessage).PopPrefixedString();

            // Only "club_habbo" is valid for this client.
            if (subscriptionName != "club_habbo")
                return;


            SubscriptionData data = sender.Subscriptions.Value[subscriptionName];

            new MSubscriptionData
            {
                SubscriptionName = subscriptionName,
                CurrentDay = data.Expired.Days % 31,
                ElapsedMonths = data.Expired.Days / 31,
                PrepaidMonths = data.Remaining.Days / 31,
                IsActive = data.IsActive
            }.Send(sender);
        }
    }
}
