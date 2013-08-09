using System;

namespace IHI.Server.Network
{
    public class BinaryGameSocketReader : GameSocketReader
    {
        public override int LengthBytes
        {
            get { return 4; }
        }

        public override int ParseLength(byte[] data)
        {
            return BinaryEncoding.DecodeInt(data);
        }

        public override IncomingMessage ParseMessage(byte[] data)
        {
            try
            {
                byte[] headerBytes = new[] { data[0], data[1] };
                int headerId = Base64Encoding.DecodeInt32(headerBytes);

                byte[] contentBytes = new byte[data.Length - 2];
                for (int i = 0; i < data.Length - 2; i++)
                {
                    contentBytes[i] = data[i + 2];
                }

                return new BinaryIncomingMessage(headerId, contentBytes);
            }
            catch (Exception e)
            {
                throw new FormatException(CoreManager.ServerCore.StringLocale.GetString("CORE:ERROR_NETWORK_INVALID_DATA"), e);
            }
        }
    }
}