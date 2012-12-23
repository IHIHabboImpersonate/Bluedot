using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public static partial class PacketHandlers
    {
        private static void ProcessBalanceRequest(Habbo sender, IncomingMessage message)
        {
            new MCreditBalance
                {
                    Balance = sender.Credits
                }.Send(sender);
        }
    }
}
