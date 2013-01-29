using System;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultMessengerFunctions
{
    public class DefaultMessengerFunctions : IPseudoPlugin
    {
        public void Start()
        {
            CoreManager.ServerCore.EventManager.StrongBind("habbo_login:after", RegisterMessengerHandlers);
        }

        private static void RegisterMessengerHandlers(object source, EventArgs args)
        {
            Habbo habbo = (Habbo)source;
            habbo.Socket.PacketHandlers[12, GameSocketMessageHandlerPriority.DefaultAction] += PacketHandlers.ProcessMessengerInit;
        }
    }
}
