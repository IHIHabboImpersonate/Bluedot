using Bluedot.HabboServer.ApiUsage.Packets;
using Bluedot.HabboServer.ApiUsage.Figures;
using Bluedot.HabboServer.ApiUsage.PermissionEnforcement;

namespace Bluedot.HabboServer.ApiUsage
{
    public static class ApiCallerRoot
    {
        public static void Start()
        {
            PacketHandlers.Start();
            FigureRoot.Start();
            PermissionEnforcementRoot.Start();
        }
    }
}
