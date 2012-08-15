using Bluedot.HabboServer.ApiUsage.Packets;
using Bluedot.HabboServer.ApiUsage.Figures;

namespace Bluedot.HabboServer.ApiUsage
{
    public static class ApiCallerRoot
    {
        public static void Start()
        {
            PacketHandlers.Start();
            FigureRoot.Start();
        }
    }
}
