using System.Collections.Generic;

namespace Bluedot.HabboServer.ApiUsage.Plugins
{
    interface IPseudoPlugin
    {
        void Start();
    }
    public static class ApiCallerRoot
    {
        public static void Start()
        {
            List<IPseudoPlugin> plugins = new List<IPseudoPlugin>();

            plugins.Add(new DefaultLoginFunctions.DefaultLoginFunctions());
            plugins.Add(new DefaultHabboFunctions.DefaultHabboFunctions());
            plugins.Add(new DefaultMessengerFunctions.DefaultMessengerFunctions());
            plugins.Add(new DefaultSubscriptionsFunctions.DefaultSubscriptionsFunctions());
            plugins.Add(new ClassicFigures.ClassicFigures());
            plugins.Add(new TestingSandbox.TestingSandbox());

            foreach (IPseudoPlugin plugin in plugins)
            {
                plugin.Start();
            }
        }
    }
}
