using IHI.Server.Events;
using System;
using System.Threading;

namespace IHI.Server.Plugins
{
    public abstract class Plugin
    {
        internal ManualResetEvent StartedResetEvent = new ManualResetEvent(false);
        public string Name { get; internal set; }

        /// <summary>
        ///   Called when the plugin is started.
        /// </summary>
        public abstract void Start(EventFirer eventFirer);

        #region Start Waiting
        public void WaitTillStarted()
        {
            StartedResetEvent.WaitOne();
        }
        public void WaitTillStarted(int millisecondsTimeout)
        {
            StartedResetEvent.WaitOne(millisecondsTimeout);
        }
        public void WaitTillStarted(TimeSpan timeout)
        {
            StartedResetEvent.WaitOne(timeout);
        }
        #endregion
    }
}
