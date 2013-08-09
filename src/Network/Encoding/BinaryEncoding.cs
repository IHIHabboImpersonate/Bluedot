#region Usings

using System;
using System.Linq;
using System.Text;

#endregion

namespace Bluedot.HabboServer.Network
{
    internal static class BinaryEncoding
    {
        #region Methods

        internal static byte[] EncodeBool(bool value)
        {
            if (value)
                return new byte[] { 1 };
            return new byte[] { 0 };
        }
        internal static byte[] EncodeByte(byte value) // byte into byte[] basically... kind of pointless...
        {
            return new byte[] { value };
        }
        internal static byte[] EncodeShort(short value)
        {
            return new byte[] { (byte)(value >> 010), (byte)value };
        }
        internal static byte[] EncodeInt(int value)
        {
            return new byte[] { (byte)(value >> 030), (byte)(value >> 020), (byte)(value >> 010), (byte)value };
        }
        internal static byte[] EncodeString(string value)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(value);
            byte[] lengthBytes = EncodeShort((short)stringBytes.Length);

            return lengthBytes.Concat(stringBytes).ToArray();
        }


        internal static bool DecodeBool(byte[] value)
        {
            return value[0] == 1;
        }
        internal static byte DecodeByte(byte[] value) // byte[] into byte basically... kind of pointless...
        {
            return value[0];
        }
        internal static short DecodeShort(byte[] value)
        {
            return (short)((value[0] << 010) | value[1]);
        }
        internal static int DecodeInt(byte[] value)
        {
            return (short)((value[0] << 030) | (value[1] << 020) | (value[2] << 010) | value[3]);
        }

        #endregion
    }
}