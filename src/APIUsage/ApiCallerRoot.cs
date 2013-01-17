using System.Collections.Generic;

namespace Bluedot.HabboServer.ApiUsage.Plugins
{
    interface ITempPlugin
    {
        void Start();
    }
    public static class ApiCallerRoot
    {
        public static void Start()
        {
            List<ITempPlugin> plugins = new List<ITempPlugin>();

            plugins.Add(new DefaultLoginFunctions.DefaultLoginFunctions());
            plugins.Add(new DefaultHabboFunctions.DefaultHabboFunctions());
            plugins.Add(new DefaultMessengerFunctions.DefaultMessengerFunctions());
            plugins.Add(new DefaultRoomEventsFunctions.DefaultRoomEventsFunctions());
            plugins.Add(new DefaultSubscriptionsFunctions.DefaultSubscriptionsFunctions());
            plugins.Add(new ClassicFigures.ClassicFigures());

            foreach (ITempPlugin plugin in plugins)
            {
                plugin.Start();
            }
        }
    }
}
