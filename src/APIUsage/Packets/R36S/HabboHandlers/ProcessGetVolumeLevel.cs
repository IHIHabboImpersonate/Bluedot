using Bluedot.HabboServer.APIUsage;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public static partial class PacketHandlers
    {
        private static void ProcessGetVolumeLevel(Habbo sender, IncomingMessage message)
        {
            new MVolumeLevel
            {
                // TODO: Should Volume really be an extension?
                Volume = sender.VolumeProperty()
            }.Send(sender);
        }
    }
}
