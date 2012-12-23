using System.ComponentModel;

using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    using System;

    public static partial class PacketHandlers
    {
        private static void ProcessSSOTicket(Habbo sender, IncomingMessage message)
        {
            ClassicIncomingMessage classicMessage = message as ClassicIncomingMessage;

            Habbo fullHabbo = CoreManager.ServerCore.HabboDistributor.GetHabboFromSSOTicket(
                classicMessage.PopPrefixedString());

            if (fullHabbo == null)
            {
                new MConnectionClosed
                {
                    Reason = ConnectionClosedReason.InvalidSSOTicket
                }.Send(sender);
                
                sender.Socket.Disconnect(); // Invalid SSO Ticket - Disconnect!
            }
            else
            {
                // If this Habbo is already logged in...
                if (fullHabbo.LoggedIn)
                {
                    // Disconnect them.
                    new MConnectionClosed
                        {
                            Reason = ConnectionClosedReason.ConcurrentLogin
                        }.Send(fullHabbo);
                    fullHabbo.Socket.Disconnect();
                }

                LoginMerge(fullHabbo, sender);
            }
        }


        #region Method: LoginMerge
        private static void LoginMerge(Habbo fullHabbo, Habbo connectionHabbo)
        {
            CancelEventArgs eventArgs = new CancelEventArgs();
            CoreManager.ServerCore.EventManager.Fire("habbo_login:before", fullHabbo, eventArgs);

            if (eventArgs.Cancel)
            {
                if (connectionHabbo.Socket != null)
                    connectionHabbo.Socket.Disconnect();
                return;
            }

            connectionHabbo.Socket.Habbo = fullHabbo;
            fullHabbo.Socket = connectionHabbo.Socket;

            fullHabbo.LastAccess = DateTime.Now;
            CoreManager.ServerCore.EventManager.Fire("habbo_login:after", fullHabbo, eventArgs);
        }
        #endregion
    }
}
