using System;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public static partial class PacketHandlers
    {
        public static void Start()
        {
            CoreManager.ServerCore.EventManager
                .StrongBind("incoming_game_connection:after", RegisterLoginHandlers)

                // Register the handlers for logged in clients.
                .StrongBind("habbo_login:after", RegisterHabboHandlers)
                .StrongBind("habbo_login:after", RegisterMessengerHandlers)
                .StrongBind("habbo_login:after", RegisterSubscriptionHandlers)

                // Inform the client of a successful login.
                .StrongBind("habbo_login:after", (source, e) => new MAuthenticationOkay().Send(source as IMessageable));
        }

        private static void RegisterLoginHandlers(object source, EventArgs args)
        {
            GameSocket socket = (GameSocket)source;

            socket.PacketHandlers[206, GameSocketMessageHandlerPriority.DefaultAction] += ProcessEncryptionRequest;
            socket.PacketHandlers[2002, GameSocketMessageHandlerPriority.DefaultAction] += ProcessSessionRequest;
            socket.PacketHandlers[204, GameSocketMessageHandlerPriority.DefaultAction] += ProcessSSOTicket;
        }
        private static void RegisterHabboHandlers(object source, EventArgs args)
        {
            Habbo habbo = (Habbo)source;
            habbo.Socket.PacketHandlers[6, GameSocketMessageHandlerPriority.DefaultAction] += ProcessBalanceRequest;
            habbo.Socket.PacketHandlers[7, GameSocketMessageHandlerPriority.DefaultAction] += ProcessHabboInfoRequest;
            habbo.Socket.PacketHandlers[8, GameSocketMessageHandlerPriority.DefaultAction] += ProcessGetVolumeLevel;
            habbo.Socket.PacketHandlers[157, GameSocketMessageHandlerPriority.DefaultAction] += ProcessBadgeListingRequest;
        }

        private static void RegisterSubscriptionHandlers(object source, EventArgs args)
        {
            Habbo habbo = (Habbo)source;
            habbo.Socket.PacketHandlers[26, GameSocketMessageHandlerPriority.DefaultAction] += ProcessSubscriptionDataRequest;
        }
        private static void RegisterMessengerHandlers(object source, EventArgs args)
        {
            Habbo habbo = (Habbo)source;
            habbo.Socket.PacketHandlers[12, GameSocketMessageHandlerPriority.DefaultAction] += ProcessMessengerInit;
        }
    }
}
