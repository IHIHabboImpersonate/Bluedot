using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultLoginFunctions
{
    internal static partial class PacketHandlers
    {
        internal static void ProcessEncryptionRequest(Habbo sender, IncomingMessage message)
        {
            new MSetupEncryption
            {
                UnknownA = false
            }.Send(sender);
        }
    }
}