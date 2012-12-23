using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    using System;

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
            GameSocketConnectionEventArgs typedArgs = args as GameSocketConnectionEventArgs;

            typedArgs.Socket.PacketHandlers[206, GameSocketMessageHandlerPriority.DefaultAction] += ProcessEncryptionRequest;
            typedArgs.Socket.PacketHandlers[2002, GameSocketMessageHandlerPriority.DefaultAction] += ProcessSessionRequest;
            typedArgs.Socket.PacketHandlers[204, GameSocketMessageHandlerPriority.DefaultAction] += ProcessSSOTicket;
        }
        private static void RegisterHabboHandlers(object source, EventArgs args)
        {
            (source as Habbo).Socket.PacketHandlers[6, GameSocketMessageHandlerPriority.DefaultAction] += ProcessBalanceRequest;
            (source as Habbo).Socket.PacketHandlers[7, GameSocketMessageHandlerPriority.DefaultAction] += ProcessHabboInfoRequest;
            (source as Habbo).Socket.PacketHandlers[8, GameSocketMessageHandlerPriority.DefaultAction] += ProcessGetVolumeLevel;
            (source as Habbo).Socket.PacketHandlers[157, GameSocketMessageHandlerPriority.DefaultAction] += ProcessBadgeListingRequest;
        }

        private static void RegisterSubscriptionHandlers(object source, EventArgs args)
        {
            (source as Habbo).Socket.PacketHandlers[26, GameSocketMessageHandlerPriority.DefaultAction] += ProcessSubscriptionDataRequest;
        }
        private static void RegisterMessengerHandlers(object source, EventArgs args)
        {
            (source as Habbo).Socket.PacketHandlers[12, GameSocketMessageHandlerPriority.DefaultAction] += ProcessMessengerInit;
        }
    }
}
