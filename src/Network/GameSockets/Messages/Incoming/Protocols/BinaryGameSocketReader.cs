using System;

namespace Bluedot.HabboServer.Network
{
    public class BinaryGameSocketReader : GameSocketReader
    {
        public override int LengthBytes
        {
            get { return 2; }
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