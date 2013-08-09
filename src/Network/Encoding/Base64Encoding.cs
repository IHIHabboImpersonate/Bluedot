#region Usings

using System;

#endregion

namespace IHI.Server.Network
{
    /// <summary>
    ///   Provides methods for encoding and decoding integers to byte arrays. This class is static.
    /// </summary>
    internal class Base64Encoding
    {
        #region Constants

        internal const byte Negative = 64; // '@'
        internal const byte Positive = 65; // 'A'

        #endregion

        #region Methods

        /// <summary>
        ///   Encodes a 32 bit integer to a Base64 byte array of a given length.
        /// </summary>
        /// <param name = "i">The integer to encode.</param>
        /// <param name = "numBytes">The length of the byte array.</param>
        /// <returns>A byte array holding the encoded integer.</returns>
        internal static byte[] EncodeInt32(Int32 i, int numBytes = 2)
        {
            byte[] bzRes = new byte[numBytes];
            for (int j = 1; j <= numBytes; j++)
            {
                int k = ((numBytes - j) * 6);
                bzRes[j - 1] = (byte)(0x40 + ((i >> k) & 0x3f));
            }

            return bzRes;
        }

        internal static byte[] EncodeuUInt32(uint i, int numBytes = 2)
        {
            return EncodeInt32((Int32)i, numBytes);
        }

        /// <summary>
        ///   Base64 decodes a byte array to a 32 bit integer.
        /// </summary>
        /// <param name = "bzData">The input byte array to decode.</param>
        /// <returns>The decoded 32 bit integer.</returns>
        internal static Int32 DecodeInt32(byte[] bzData)
        {
            int i = 0;
            int j = 0;
            for (int k = bzData.Length - 1; k >= 0; k--)
            {
                int x = bzData[k] - 0x40;
                if (j > 0)
                    x *= (int)Math.Pow(64.0, j);

                i += x;
                j++;
            }

            return i;
        }

        internal static uint DecodeUInt32(byte[] bzData)
        {
            return (uint)DecodeInt32(bzData);
        }

        #endregion
    }
}