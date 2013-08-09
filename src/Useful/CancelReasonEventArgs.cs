using System.ComponentModel;

namespace IHI.Server.Useful
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
