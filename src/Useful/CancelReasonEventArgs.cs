using System.ComponentModel;

namespace Bluedot.HabboServer.Useful
{
    public class CancelReasonEventArgs : CancelEventArgs
    {
        public string CancelReason
        {
            get;
            set;
        }
    }
}
