using System;
using System.Collections.Generic;

using Bluedot.HabboServer.Events;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Permissions;
using Bluedot.HabboServer.Useful;
using System.Net;
using Bluedot.HabboServer.Network.GameSockets;

namespace Bluedot.HabboServer.ApiUsage.Plugins.SomeFlashProtocol
{
    public class SomeFlashProtocol : IPseudoPlugin
    {
        public void Start(EventFirer eventFirer)
        {
            GameSocketManager gameSocketManager = CoreManager.ServerCore.NewGameSocketManager("SomeFlashProtocol", 14478, new GameSocketProtocol(new Version(63, -1), new BinaryGameSocketReader()));
            if (gameSocketManager != null)
                gameSocketManager.Start();
        }
    }
}
