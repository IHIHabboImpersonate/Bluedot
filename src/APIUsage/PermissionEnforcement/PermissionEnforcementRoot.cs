using Bluedot.HabboServer;

namespace Bluedot.HabboServer.ApiUsage.PermissionEnforcement
{
    public static class PermissionEnforcementRoot
    {
        public static void Start()
        {
            CoreManager.ServerCore.EventManager.Bind("habbo_login:before", LoginEnforcement.EnforceLoginPermission);
        }
    }
}
