using Bluedot.HabboServer.Events;

namespace Bluedot.HabboServer.ApiUsage.Plugins
{
    interface IPseudoPlugin
    {
        void Start(EventFirer eventFirer);
    }
}
