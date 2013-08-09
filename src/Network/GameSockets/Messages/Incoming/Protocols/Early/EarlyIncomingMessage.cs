using System;

namespace IHI.Server.Network
{
    internal class EarlyIncomingMessage : IncomingMessage
    {
        #region Header Related
        /// <summary>
        /// Gets the integer representation of the header.
        /// </summary>
        public override int HeaderId
        {
            get { throw new NotImplementedException(); }
        }

        private readonly string _headerString = "";

        /// <summary>
        /// Gets the string representation of the header.
        /// </summary>
        public override string HeaderString
        {
            get { return _headerString; }
        }
        #endregion
    }
}