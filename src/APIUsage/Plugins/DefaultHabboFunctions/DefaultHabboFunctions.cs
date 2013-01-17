using System;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultHabboFunctions
{
    public class DefaultHabboFunctions : ITempPlugin
    {
        public void Start()
        {
            CoreManager.ServerCore.EventManager.StrongBind("habbo_login:after", RegisterHabboHandlers);
        }

        private static void RegisterHabboHandlers(object source, EventArgs args)
        {
            Habbo habbo = (Habbo)source;
            habbo.Socket.PacketHandlers[6, GameSocketMessageHandlerPriority.DefaultAction] += PacketHandlers.ProcessBalanceRequest;
            habbo.Socket.PacketHandlers[7, GameSocketMessageHandlerPriority.DefaultAction] += PacketHandlers.ProcessHabboInfoRequest;
            habbo.Socket.PacketHandlers[8, GameSocketMessageHandlerPriority.DefaultAction] += PacketHandlers.ProcessGetVolumeLevel;
            habbo.Socket.PacketHandlers[157, GameSocketMessageHandlerPriority.DefaultAction] += PacketHandlers.ProcessBadgeListingRequest;
        }
    }
}
