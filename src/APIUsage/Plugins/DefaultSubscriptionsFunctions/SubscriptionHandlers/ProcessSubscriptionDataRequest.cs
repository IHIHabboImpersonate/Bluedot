using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultSubscriptionsFunctions
{
    internal static partial class PacketHandlers
    {
        internal static void ProcessSubscriptionDataRequest(Habbo sender, IncomingMessage message)
        {
            ClassicIncomingMessage classicMessage = (ClassicIncomingMessage)message;
            string subscriptionName = classicMessage.PopPrefixedString();

            // Only "club_habbo" is valid for this client.
            if (subscriptionName != "club_habbo")
                return;


            SubscriptionData data = sender.Subscriptions[subscriptionName];

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
