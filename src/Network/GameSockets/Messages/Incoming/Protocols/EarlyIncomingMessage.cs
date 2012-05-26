using System;

namespace Bluedot.HabboServer.Network
{
    public class EarlyIncomingMessage : IncomingMessage
    {
        #region Header Related
        /// <summary>
        /// Gets the integer representation of the header.
        /// </summary>
        public override int HeaderId
        {
            get { throw new NotImplementedException(); }
        }

        private string _headerString = "";
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