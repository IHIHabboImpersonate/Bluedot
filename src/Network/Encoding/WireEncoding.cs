#region Usings

using System;

#endregion

namespace IHI.Server.Network
{
    internal static class WireEncoding
    {
        #region Fields

        internal const byte Negative = 72; // 'H'
        internal const byte Positive = 73; // 'I'
        internal const int MaxIntegerByteAmount = 6;

        #endregion

        #region Methods

        internal static byte[] EncodeInt32(Int32 i)
        {
            byte[] wf = new byte[MaxIntegerByteAmount];
            int pos = 0;
            int numBytes = 1;
            int startPos = pos;
            int negativeMask = i >= 0 ? 0 : 4;
            i = Math.Abs(i);
            wf[pos++] = (byte)(64 + (i & 3));
            for (i >>= 2; i != 0; i >>= MaxIntegerByteAmount)
            {
                numBytes++;
                wf[pos++] = (byte)(64 + (i & 0x3f));
            }
            wf[startPos] = (byte)(wf[startPos] | numBytes << 3 | negativeMask);

            // Skip the null bytes in the result
            byte[] bzData = new byte[numBytes];
            for (int x = 0; x < numBytes; x++)
            {
                bzData[x] = wf[x];
            }

            return bzData;
        }

        internal static Int32 DecodeInt32(byte[] bzData, out Int32 totalBytes)
        {
            int pos = 0;
            bool negative = (bzData[pos] & 4) == 4;
            totalBytes = bzData[pos] >> 3 & 7;
            int v = bzData[pos] & 3;
            pos++;
            int shiftAmount = 2;
            for (int b = 1; b < totalBytes; b++)
            {
                v |= (bzData[pos] & 0x3f) << shiftAmount;
                shiftAmount = 2 + 6 * b;
                pos++;
            }

            if (negative)
                v *= -1;

            return v;
        }

        #endregion
    }
}