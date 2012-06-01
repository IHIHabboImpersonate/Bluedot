using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public static partial class PacketHandlers
    {
        private static void ProcessEncryptionRequest(Habbo sender, IncomingMessage message)
        {
            new MSetupEncryption
            {
                UnknownA = false
            }.Send(sender);
        }
    }
}
