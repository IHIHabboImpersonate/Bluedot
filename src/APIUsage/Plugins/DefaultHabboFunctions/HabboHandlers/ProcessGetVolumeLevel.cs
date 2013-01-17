using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultHabboFunctions
{
    internal static partial class PacketHandlers
    {
        internal static void ProcessGetVolumeLevel(Habbo sender, IncomingMessage message)
        {
            new MVolumeLevel
            {
                // TODO: Should Volume really be an extension?
                Volume = sender.VolumeProperty()
            }.Send(sender);
        }
    }
}
