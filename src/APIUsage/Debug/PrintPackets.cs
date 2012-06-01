using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;
using Nito.Async;

namespace Bluedot.HabboServer.ApiUsage
{
    public static class PrintPackets
    {
        public static void Start()
        {
            CoreManager.ServerCore.GameSocketManager.OnPostIncomingConnection += RegisterHandlers;
        }

        public static void RegisterHandlers(object source, GameSocketConnectionEventArgs args)
        {
            args.Socket.PacketArrived += PrintIncomingPacketData;
            Habbo.OnAnyMessageSent += PrintOutgoingPacketData;
        }

        private static void PrintOutgoingPacketData(object sender, GameSocketMessageEventArgs args)
        {
            CoreManager.ServerCore.StandardOut.PrintDebug("OUTGOING => " + args.Message.Header + args.Message.ContentString);
        }

        public static void PrintIncomingPacketData(AsyncResultEventArgs<byte[]> data)
        {
            if(data.Result != null)
                CoreManager.ServerCore.StandardOut.PrintDebug("INCOMING => " + data.Result.ToUtf8String());
        }
    }
}
