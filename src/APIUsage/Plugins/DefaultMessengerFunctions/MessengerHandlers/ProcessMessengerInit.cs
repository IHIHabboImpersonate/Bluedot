using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultMessengerFunctions
{
    internal static partial class PacketHandlers
    {
        internal static void ProcessMessengerInit(Habbo sender, IncomingMessage message)
        {
            // TODO: Load friends and categories.

            new MMessengerInit
            {
                Categories = sender.MessengerCategories,
                UnknownA = 10,
                UnknownB = 20,
                UnknownC = 30,
            }.Send(sender);
        }
    }
}
