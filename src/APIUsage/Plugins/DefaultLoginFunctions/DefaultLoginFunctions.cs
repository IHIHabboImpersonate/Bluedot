using System;
using System.Collections.Generic;

using Bluedot.HabboServer.Events;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Permissions;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultLoginFunctions
{
    public class DefaultLoginFunctions : IPseudoPlugin
    {
        public void Start()
        {
            CoreManager.ServerCore.EventManager.StrongBind("incoming_game_connection", EventPriority.After, RegisterLoginHandlers)

                // Inform the client of a successful login.
                .StrongBind("habbo_login", EventPriority.After, ConfirmLogin)
                .StrongBind("habbo_login", EventPriority.After, SendFuseRights)
                .StrongBind("fuseright_request", EventPriority.Before, RegisterFuseRight);
        }

        private void ConfirmLogin(object source, EventArgs e)
        {
            IMessageable sender = (IMessageable)source;
            new MAuthenticationOkay().Send(sender);
        }

        private void SendFuseRights(object source, EventArgs e)
        {
            Habbo sender = (Habbo)source;

            FuseRightEventArgs fuseArgs = new FuseRightEventArgs();


            CoreManager.ServerCore.EventManager.Fire("fuseright_request", EventPriority.Before, sender, fuseArgs);

            new MFuseRights
                {
                    FuseRights = fuseArgs.GetFuseRights()
                }.Send(sender);

            CoreManager.ServerCore.EventManager.Fire("fuseright_request", EventPriority.After, sender, fuseArgs);
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

        public void RegisterFuseRight(object source, EventArgs args)
        {
            FuseRightEventArgs fuseArgs = (FuseRightEventArgs)args;
            fuseArgs.AddFuseRight("fuse_login");
        }
    }
}
