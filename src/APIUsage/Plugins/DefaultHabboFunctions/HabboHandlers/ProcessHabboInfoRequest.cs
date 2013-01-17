using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultHabboFunctions
{
    internal static partial class PacketHandlers
    {
        internal static void ProcessHabboInfoRequest(Habbo sender, IncomingMessage message)
        {
            new MHabboData
            {
                HabboId = sender.Id,
                Username = sender.Username,
                Motto = sender.Motto,
                Figure = sender.Figure
            }.Send(sender);
        }
    }
}
