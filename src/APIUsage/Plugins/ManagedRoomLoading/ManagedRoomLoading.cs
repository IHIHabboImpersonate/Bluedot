using System;

using Bluedot.HabboServer.APIUsage.Plugins.ManagedRoomLoading;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Rooms;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.ApiUsage.Plugins.ManagedRoomLoading
{
    public class ManagedRoomLoading : IPseudoPlugin
    {
        public void Start()
        {
            CoreManager.ServerCore.RoomDistributor.SetOverrideLoader(0, habbo => habbo.GetRoomManaged());
        }

        private static void SendToVirtualRoom(object source, EventArgs args)
        {
            Habbo habbo = (Habbo)source;
        }
    }
}
