using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;
using Nito.Async;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public static partial class PacketHandlers
    {
        public static void Start()
        {
            CoreManager.ServerCore.GameSocketManager.OnPostIncomingConnection += RegisterHandlers;
            Habbo.OnAnyLogin += SendAuthenticationOkay;
        }

        private static void RegisterHandlers(object source, GameSocketConnectionEventArgs args)
        {
            args.Socket.PacketHandlers[206, GameSocketMessageHandlerPriority.DefaultAction] += ProcessEncryptionRequest;
            args.Socket.PacketHandlers[2002, GameSocketMessageHandlerPriority.DefaultAction] += ProcessSessionRequest;
            args.Socket.PacketHandlers[204, GameSocketMessageHandlerPriority.DefaultAction] += ProcessSSOTicket;
        }
    }
}
