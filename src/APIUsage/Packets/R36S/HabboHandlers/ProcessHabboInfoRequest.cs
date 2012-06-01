using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public static partial class PacketHandlers
    {
        private static void ProcessHabboInfoRequest(Habbo sender, IncomingMessage message)
        {
            new MHabboData
            {
                HabboID = sender.Id,
                Username = sender.Username,
                Motto = sender.Motto,
                Figure = sender.Figure
            }.Send(sender);
        }
    }
}
