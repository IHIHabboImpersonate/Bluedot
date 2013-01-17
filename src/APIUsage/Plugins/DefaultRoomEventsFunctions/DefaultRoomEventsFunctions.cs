using System;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultRoomEventsFunctions
{
    public class DefaultRoomEventsFunctions : ITempPlugin
    {
        public void Start()
        {
            CoreManager.ServerCore.EventManager.StrongBind("habbo_login:after", RegisterRoomEventHandlers);
        }

        private static void RegisterRoomEventHandlers(object source, EventArgs args)
        {
            Habbo habbo = (Habbo)source;
            habbo.Socket.PacketHandlers[315, GameSocketMessageHandlerPriority.DefaultAction] += PacketHandlers.ProcessEventCategoryValidationRequest;
        }
    }
}
