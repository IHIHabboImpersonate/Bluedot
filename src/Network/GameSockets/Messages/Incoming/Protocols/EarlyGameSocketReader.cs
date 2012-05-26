using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bluedot.HabboServer.Network
{
    public class EarlyGameSocketReader : GameSocketReader
    {
        public override int LengthBytes
        {
            get { throw new NotImplementedException(); }
        }

        public override int ParseLength(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override IncomingMessage ParseMessage(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}