using System;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultSubscriptionsFunctions
{
    public class DefaultSubscriptionsFunctions : IPseudoPlugin
    {
        public void Start()
        {
            CoreManager.ServerCore.EventManager.StrongBind("habbo_login:after", RegisterSubscriptionHandlers);
        }

        private static void RegisterSubscriptionHandlers(object source, EventArgs args)
        {
            Habbo habbo = (Habbo)source;
            habbo.Socket.PacketHandlers[26, GameSocketMessageHandlerPriority.DefaultAction] += PacketHandlers.ProcessSubscriptionDataRequest;
        }
    }
}
