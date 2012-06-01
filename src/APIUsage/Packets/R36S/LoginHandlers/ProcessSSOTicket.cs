using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public static partial class PacketHandlers
    {
        private static void ProcessSSOTicket(Habbo sender, IncomingMessage message)
        {
            ClassicIncomingMessage classicMessage = message as ClassicIncomingMessage;

            Habbo loggedInHabbo = CoreManager.ServerCore.HabboDistributor.GetHabbo(
                classicMessage.PopPrefixedString(), 
                sender.Socket.IPAddress);

            if (loggedInHabbo == null)
            {
                new MConnectionClosed
                {
                    Reason = ConnectionClosedReason.InvalidSSOTicket
                }.Send(sender);
                
                sender.Socket.Disconnect(); // Invalid SSO Ticket - Disconnect!
            }
            else
            {

                lock (loggedInHabbo)
                {
                    // If this Habbo is already logged in...
                    if (loggedInHabbo.LoggedIn)
                    {
                        // Disconnect them.
                        new MConnectionClosed
                            {
                                Reason = ConnectionClosedReason.ConcurrentLogin
                            }.Send(loggedInHabbo);
                        loggedInHabbo.Socket.Disconnect();
                        loggedInHabbo.LoggedIn = false;
                    }

                    loggedInHabbo.LoginMerge(sender);
                    loggedInHabbo.LoggedIn = true;
                }
            }
        }
    }
}
