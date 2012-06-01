using System;

namespace Bluedot.HabboServer.Network
{
    public class ClassicGameSocketReader : GameSocketReader
    {
        public override int LengthBytes
        {
            get { return 3; }
        }

        public override int ParseLength(byte[] data)
        {
            return Base64Encoding.DecodeInt32(data);
        }

        public override IncomingMessage ParseMessage(byte[] data)
        {
            try
            {
                byte[] headerBytes = new [] {data[0], data[1]};
                int headerId = Base64Encoding.DecodeInt32(headerBytes);

                byte[] contentBytes = new byte[data.Length-2];
                for(int i = 0; i < data.Length-2; i++)
                {
                    contentBytes[i] = data[i + 2];
                }

                return new ClassicIncomingMessage(headerId, contentBytes);
            }
            catch (Exception e)
            {
                throw new FormatException(
                    "Invalid packet data. Are you sure you are using the correct GameSocketReader?", e);
            }
        }
    }
}