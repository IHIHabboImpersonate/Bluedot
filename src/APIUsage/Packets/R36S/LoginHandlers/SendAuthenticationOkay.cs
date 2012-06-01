using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public static partial class PacketHandlers
    {
        private static void SendAuthenticationOkay(object source, HabboEventArgs e)
        {
            // Inform the client of a successful login.
            new MAuthenticationOkay().
                Send(source as IMessageable);
        }
    }
}
