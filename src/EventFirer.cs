using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Bluedot.HabboServer.Useful;
using Bluedot.HabboServer.ApiUsage.Plugins;
using Bluedot.HabboServer.Events;

namespace Bluedot.HabboServer.ApiUsage.Plugins
{
    public class EventFirer
    {
        private IPseudoPlugin _plugin;

        internal EventFirer(IPseudoPlugin plugin)
        {
            _plugin = plugin;
        }

        #region Method: Fire
        internal EventFirer Fire(string eventName, EventPriority priority, object source, EventArgs args)
        {
            CoreManager.ServerCore.EventManager.Fire(_plugin, eventName, priority, source, args);
            return this;
        }
        #endregion
    }
}