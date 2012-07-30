using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public static partial class PacketHandlers
    {
        public static void Start()
        {
            CoreManager.ServerCore.GameSocketManager.OnPostIncomingConnection += RegisterLoginHandlers;
            Habbo.OnAnyLogin += SendAuthenticationOkay;
            Habbo.OnAnyLogin += RegisterHabboHandlers;
            Habbo.OnAnyLogin += RegisterMessengerHandlers;
        }

        private static void RegisterLoginHandlers(object source, GameSocketConnectionEventArgs args)
        {
            args.Socket.PacketHandlers[206, GameSocketMessageHandlerPriority.DefaultAction] += ProcessEncryptionRequest;
            args.Socket.PacketHandlers[2002, GameSocketMessageHandlerPriority.DefaultAction] += ProcessSessionRequest;
            args.Socket.PacketHandlers[204, GameSocketMessageHandlerPriority.DefaultAction] += ProcessSSOTicket;
        }
        private static void RegisterHabboHandlers(object source, HabboEventArgs args)
        {
            (source as Habbo).Socket.PacketHandlers[6, GameSocketMessageHandlerPriority.DefaultAction] += ProcessBalanceRequest;
            (source as Habbo).Socket.PacketHandlers[7, GameSocketMessageHandlerPriority.DefaultAction] += ProcessHabboInfoRequest;
            (source as Habbo).Socket.PacketHandlers[8, GameSocketMessageHandlerPriority.DefaultAction] += ProcessGetVolumeLevel;
            (source as Habbo).Socket.PacketHandlers[157, GameSocketMessageHandlerPriority.DefaultAction] += ProcessBadgeListingRequest;
        }
        private static void RegisterMessengerHandlers(object source, HabboEventArgs args)
        {
            (source as Habbo).Socket.PacketHandlers[12, GameSocketMessageHandlerPriority.DefaultAction] += ProcessMessengerInit;
        }
    }
}
