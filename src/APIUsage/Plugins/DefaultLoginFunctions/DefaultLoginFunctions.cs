using System;
using System.Collections.Generic;

using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Permissions;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultLoginFunctions
{
    public class DefaultLoginFunctions : ITempPlugin
    {
        public void Start()
        {
            CoreManager.ServerCore.EventManager.StrongBind("incoming_game_connection:after", RegisterLoginHandlers)

                // Inform the client of a successful login.
                .StrongBind("habbo_login:after", ConfirmLogin)
                .StrongBind("habbo_login:after", SendFuseRights);

            FuseRightManager fuseManager = CoreManager.ServerCore.FuseRightManager;
            
            fuseManager.RegisterFuseRight("fuse_login", (s, collection) => true);
        }

        private void ConfirmLogin(object source, EventArgs e)
        {
            IMessageable sender = (IMessageable)source;
            new MAuthenticationOkay().Send(sender);
        }

        private void SendFuseRights(object source, EventArgs e)
        {
            Habbo sender = (Habbo)source;
            
            new MFuseRights
                {
                    FuseRights = new List<string>(CoreManager.ServerCore.FuseRightManager.ResolvePermissions(sender.Permissions))
                }.Send(sender);
        }

        private static void RegisterLoginHandlers(object source, EventArgs args)
        {
            GameSocket socket = (GameSocket)source;

            socket.PacketHandlers[206, GameSocketMessageHandlerPriority.DefaultAction] += PacketHandlers.ProcessEncryptionRequest;
            socket.PacketHandlers[2002, GameSocketMessageHandlerPriority.DefaultAction] += PacketHandlers.ProcessSessionRequest;
            socket.PacketHandlers[204, GameSocketMessageHandlerPriority.DefaultAction] += PacketHandlers.ProcessSSOTicket;
        }

        private static void EnforceLoginPermission(object sender, EventArgs eventArgs)
        {
            Habbo habbo = (Habbo)sender;
            CancelReasonEventArgs cancelReasonEventArgs = (CancelReasonEventArgs)eventArgs;
            if (!habbo.HasPermission("habbo_login"))
            {
                cancelReasonEventArgs.Cancel = true;
                cancelReasonEventArgs.CancelReason = "Permission \"habbo_login\" missing!";
            }
        }
    }
}
