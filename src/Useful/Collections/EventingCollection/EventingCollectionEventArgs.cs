using System;

namespace Bluedot.HabboServer.Useful
{
    public class EventingCollectionEventArgs<T> : EventArgs
    {
        public T Item
        {
            get;
            private set;
        }
        public EventingCollectionAction Action
        {
            get;
            private set;
        }

        #region Method: EventingCollectionEventArgs (Constructor)
        public EventingCollectionEventArgs(T item, EventingCollectionAction action)
        {
            Item = item;
            Action = action;
        }
        #endregion

        public bool Cancelled
        {
            get;
            private set;
        }

        public void Cancel()
        {
            Cancelled = true;
        }
    }
}