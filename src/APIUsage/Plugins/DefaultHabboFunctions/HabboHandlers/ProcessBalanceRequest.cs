using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultHabboFunctions
{
    internal static partial class PacketHandlers
    {
        internal static void ProcessBalanceRequest(Habbo sender, IncomingMessage message)
        {
            new MCreditBalance
                {
                    Balance = sender.Credits
                }.Send(sender);
        }
    }
}
