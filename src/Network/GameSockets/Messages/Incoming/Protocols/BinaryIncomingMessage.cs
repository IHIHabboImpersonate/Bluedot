using System;

namespace Bluedot.HabboServer.Network
{
    public class BinaryIncomingMessage : IncomingMessage
    {
        #region Header Related
#pragma warning disable 649
        private int _headerId;
#pragma warning restore 649
        /// <summary>
        /// Gets the integer representation of the header.
        /// </summary>
        public override int HeaderId
        {
            get { return _headerId; }
        }

        /// <summary>
        /// Gets the string representation of the header.
        /// </summary>
        public override string HeaderString
        {
            get { throw new NotSupportedException(); }
        }
        #endregion
        
        #region Read Related
        /// <summary>
        /// Reads a length-prefixed value from the message and returns it as a byte array.
        /// </summary>
        private static byte[] PopPrefixedValue()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads a single byte as a boolean and returns it. False is returned if there is no remaining content.
        /// </summary>
        internal bool PopBoolean()
        {
            throw new NotImplementedException();
        }

        internal int PopInt32()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads a length prefixed string from the message content and encodes it with UTF8.
        /// </summary>
// ReSharper disable MemberCanBeMadeStatic.Global
        internal string PopPrefixedString()
// ReSharper restore MemberCanBeMadeStatic.Global
        {
            return PopPrefixedValue().ToUtf8String();
        }

        /// <summary>
        /// Reads a length prefixed string 32 bit integer from the message content and tries to parse it to integer. No exceptions are thrown if parsing fails.
        /// </summary>
        internal int PopPrefixedInt32()
        {
            int i;
            int.TryParse(PopPrefixedString(), out i);

            return i;
        }
        #endregion
    }
}