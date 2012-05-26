using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bluedot.HabboServer
{
    public static class Extensions
    {
        public static string ToUtf8String(this byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }
        public static byte[] ToUtf8Bytes(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        private static readonly sbyte[] HexLookup = new sbyte[]{0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46};
        public static string ToHexString(this byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 2);

            for (int buc = 0; buc < data.Length; buc++)
            {
                sb.Append(HexLookup[(data[buc] >> 4) & 0x0F]);
                sb.Append(HexLookup[data[buc] & 0x0F]);
            }

            return sb.ToString();
        }
    }
}
