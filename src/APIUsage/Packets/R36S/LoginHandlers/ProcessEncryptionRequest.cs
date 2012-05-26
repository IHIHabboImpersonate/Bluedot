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
        private static void ProcessEncryptionRequest(Habbo sender, IncomingMessage message)
        {
            new MSetupEncryption
            {
                UnknownA = false
            }.Send(sender);
        }
    }
}
