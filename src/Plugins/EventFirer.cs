using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IHI.Server.Useful;
using IHI.Server.Plugins;
using IHI.Server.Events;

namespace IHI.Server.Plugins
{
    public class EventFirer
    {
        private Plugin _plugin;

        internal EventFirer(Plugin plugin)
        {
            _plugin = plugin;
        }

        #region Method: Fire
        public EventFirer Fire(string eventName, EventPriority priority, object source, EventArgs args)
        {
            CoreManager.ServerCore.EventManager.Fire(_plugin, eventName, priority, source, args);
            return this;
        }
        #endregion
    }
}