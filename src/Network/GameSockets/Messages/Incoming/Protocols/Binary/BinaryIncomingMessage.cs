using System;
using System.Text;

namespace IHI.Server.Network
{
    public class BinaryIncomingMessage : IncomingMessage
    {
        #region Header Related
        private int _headerId;
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
            get { return "HEADER-" + _headerId; }
        }
        #endregion
        
        internal BinaryIncomingMessage(int headerId, byte[] content)
        {
            _headerId = headerId;
            Content = content;
        }

        #region Read Related
        /// <summary>
        /// Reads a single byte as a boolean and returns it. False is returned if there is no remaining content.
        /// </summary>
        private bool PopBoolean()
        {
            return BinaryEncoding.DecodeBool(ReadBytes(1));
        }
        internal byte PopByte()
        {
            return BinaryEncoding.DecodeByte(ReadBytes(1));
        }
        internal short PopShort()
        {
            return BinaryEncoding.DecodeShort(ReadBytes(2));
        }
        internal int PopInteger()
        {
            return BinaryEncoding.DecodeInt(ReadBytes(4));
        }
        internal string PopString()
        {
            short length = PopShort();
            return Encoding.UTF8.GetString(ReadBytes(length));
        }
        #endregion
    }
}