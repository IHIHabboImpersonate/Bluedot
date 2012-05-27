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
        private static void ProcessBalanceRequest(Habbo sender, IncomingMessage message)
        {
            new MCreditBalance
                {
                    Balance = sender.Credits
                }.Send(sender);
        }
    }
}
