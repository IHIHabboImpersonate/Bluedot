using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public static partial class PacketHandlers
    {
        private static void ProcessMessengerInit(Habbo sender, IncomingMessage message)
        {
            // TODO: Load friends and categories.

            new MMessengerInit
            {
                Categories = sender.MessengerCategories.Value,
                UnknownA = 10,
                UnknownB = 20,
                UnknownC = 30,
            }.Send(sender);
        }
    }
}
