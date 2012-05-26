using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bluedot.HabboServer.ApiUsage.Packets;
using Bluedot.HabboServer.Network;
using Nito.Async;

namespace Bluedot.HabboServer.ApiUsage
{
    public static class ApiCallerRoot
    {
        public static void Start()
        {
            PacketHandlers.Start();
            PrintPackets.Start();
        }
    }
}
