using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.APIUsage.Libraries.Navigator
{
    public class ListingEventArgs : CancelReasonEventArgs
    {
        #region Properties
        #region Property: Listing
        /// <summary>
        /// 
        /// </summary>
        public Listing Listing
        {
            get;
            private set;
        }
        #endregion
        #endregion

        #region Methods
        #region Method: ListingEventArgs (Constructor)
        public ListingEventArgs(Listing listing)
        {
            Listing = listing;
        }
        #endregion
        #endregion
    }
}