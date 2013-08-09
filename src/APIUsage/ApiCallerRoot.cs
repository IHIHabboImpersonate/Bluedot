using IHI.Server.Events;
using System.Collections.Generic;

namespace IHI.Server.Plugins
{
    public static class ApiCallerRoot
    {
        public static void Start()
        {
            List<Plugin> plugins = new List<Plugin>();

            //plugins.Add(new DefaultLoginFunctions.DefaultLoginFunctions());
            //plugins.Add(new DefaultHabboFunctions.DefaultHabboFunctions());
            //plugins.Add(new DefaultMessengerFunctions.DefaultMessengerFunctions());
            //plugins.Add(new DefaultSubscriptionsFunctions.DefaultSubscriptionsFunctions());
            plugins.Add(new ClassicFigures.ClassicFigures());

            foreach (Plugin plugin in plugins)
            {
                EventFirer eventFirer = new EventFirer(plugin);
                plugin.Start(eventFirer);
            }
        }
    }
}
