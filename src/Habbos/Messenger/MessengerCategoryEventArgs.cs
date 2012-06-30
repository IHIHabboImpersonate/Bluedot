using System;

namespace Bluedot.HabboServer.Habbos
{
    public class MessengerCategoryEventArgs : EventArgs
    {
        public bool Cancelled { get; private set; }

        public void Cancel()
        {
            Cancelled = true;
        }
    }
}