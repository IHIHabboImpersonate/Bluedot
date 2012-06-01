using Bluedot.HabboServer.ApiUsage.Packets;

namespace Bluedot.HabboServer.ApiUsage
{
    public static class ApiCallerRoot
    {
        public static void Start()
        {
            PacketHandlers.Start();
            PrintPackets.Start();
        }
    }
}
