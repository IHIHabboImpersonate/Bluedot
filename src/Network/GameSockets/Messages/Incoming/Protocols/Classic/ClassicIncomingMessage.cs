namespace IHI.Server.Network
{
    public class ClassicIncomingMessage : IncomingMessage
    {
        #region Header Related
        private readonly int _headerId;
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
            get { return Base64Encoding.EncodeInt32(_headerId).ToUtf8String(); }
        }
        #endregion

        internal ClassicIncomingMessage(int headerId, byte[] content)
        {
            _headerId = headerId;
            Content = content;
        }

        #region Read Related
        /// <summary>
        /// Reads a length-prefixed (Base64) value from the message and returns it as a byte array.
        /// </summary>
        private byte[] PopPrefixedValue()
        {
            int length = Base64Encoding.DecodeInt32(ReadBytes(2));
            return ReadBytes(length);
        }

        /// <summary>
        /// Reads a Base64 boolean and returns it. False is returned if there is no remaining content.
        /// </summary>
        internal bool PopBase64Boolean()
        {
            return (RemainingContent > 0 && Content[_contentCursor++] == Base64Encoding.Positive);
        }

        internal int PopBase64Int32()
        {
            return Base64Encoding.DecodeInt32(ReadBytes(2));
        }

        /// <summary>
        /// Reads a length prefixed string from the message content and encodes it with UTF8.
        /// </summary>
        internal string PopPrefixedString()
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

        /// <summary>
        /// Reads a wire format boolean and returns it. False is returned if there is no remaining content.
        /// </summary>
        internal bool PopWiredBoolean()
        {
            return (RemainingContent > 0 && Content[_contentCursor++] == WireEncoding.Positive);
        }
        /// <summary>
        /// Reads the next wire encoded 32 bit integer from the message content and advances the reader cursor.
        /// </summary>
        internal int PopWiredInt32()
        {
            if (RemainingContent == 0)
                return 0;

            byte[] bzData = ReadBytesFreezeCursor(WireEncoding.MaxIntegerByteAmount);
            int totalBytes;
            int i = WireEncoding.DecodeInt32(bzData, out totalBytes);
            _contentCursor += totalBytes;

            return i;
        }
        #endregion
    }
}