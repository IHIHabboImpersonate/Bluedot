namespace IHI.Server.Network
{
    public abstract class IncomingMessage
    {
        #region Header Related
        /// <summary>
        /// Gets the integer representation of the header.
        /// </summary>
        public abstract int HeaderId
        {
            get;
        }
        /// <summary>
        /// Gets the string representation of the header.
        /// </summary>
        public abstract string HeaderString
        {
            get;
        }
        #endregion

        #region Content Related
        /// <summary>
        /// The content of this message as a byte array.
        /// </summary>
        public byte[] Content
        {
            get;
            protected set;
        }
        /// <summary>
        /// Returns the total content of this message as a string.
        /// </summary>
        public string ContentString
        {
            get { return Content.ToUtf8String(); }
        }

        /// <summary>
        /// Gets the amount of unread content bytes.
        /// </summary>
        public int RemainingContent
        {
            get { return Content.Length - _contentCursor; }
        }

        /// <summary>
        /// The current index in the content array, used when reading the message.
        /// </summary>
// ReSharper disable InconsistentNaming
        protected int _contentCursor;
// ReSharper restore InconsistentNaming
        #endregion

        /// <summary>
        /// If true then lower priority handlers will not be invoked.
        /// </summary>
        public bool Cancelled
        {
            get;
            private set;
        }

        #region Read Related
        /// <summary>
        /// Resets the client message to it's state when it was constructed by resetting the content reader cursor. This allows to re-read read data.
        /// </summary>
        public IncomingMessage Reset()
        {
            _contentCursor = 0;
            return this;
        }

        /// <summary>
        /// Advances the content cursor by a given amount of bytes.
        /// </summary>
        /// <param name="n">The amount of bytes to 'skip'.</param>
        public IncomingMessage Advance(int n)
        {
            _contentCursor += n;
            return this;
        }

        /// <summary>
        /// Reads a given amount of bytes from the remaining message content and returns it in a byte array. The reader cursor is incremented during reading.
        /// </summary>
        /// <param name="numBytes">The amount of bytes to read, advance and return. If there is less remaining data than this value, all remaining data will be read.</param>
        /// <returns>byte[]</returns>
        protected byte[] ReadBytes(int numBytes)
        {
            if (numBytes > RemainingContent)
                numBytes = RemainingContent;

            byte[] bzData = new byte[numBytes];
            for (int x = 0; x < numBytes; x++)
            {
                bzData[x] = Content[_contentCursor++];
            }

            return bzData;
        }
        /// <summary>
        /// Reads a given amount of bytes from the remaining message content and returns it in a byte array. The reader cursor does not increment during reading.
        /// </summary>
        /// <param name="numBytes">The amount of bytes to read, advance and return. If there is less remaining data than this value, all remaining data will be read.</param>
        /// <returns>byte[]</returns>
        protected byte[] ReadBytesFreezeCursor(int numBytes)
        {
            if (numBytes > RemainingContent)
                numBytes = RemainingContent;

            byte[] bzData = new byte[numBytes];
            for (int x = 0, y = _contentCursor; x < numBytes; x++, y++)
            {
                bzData[x] = Content[y];
            }

            return bzData;
        }
        #endregion

        public IncomingMessage Cancel()
        {
            Cancelled = true;
            return this;
        }
    }
}