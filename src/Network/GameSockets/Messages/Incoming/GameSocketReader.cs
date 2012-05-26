using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bluedot.HabboServer.Network
{
    public abstract class GameSocketReader
    {
        public abstract int LengthBytes { get; }
        public abstract int ParseLength(byte[] data);
        public abstract IncomingMessage ParseMessage(byte[] data);
    }
}