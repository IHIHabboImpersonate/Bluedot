using System;
using Bluedot.HabboServer.Events;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Rooms;
using Bluedot.HabboServer.Useful;
using Bluedot.HabboServer.APIUsage.Plugins.ManagedRoomLoading;

namespace Bluedot.HabboServer.ApiUsage.Plugins.TestingSandbox
{
    public class TestingSandbox : IPseudoPlugin
    {
        private static Room _virtualTest;
        public void Start()
        {
            CoreManager.ServerCore.RoomDistributor.AddOverrideLoader(GetVirtualTest);

            CoreManager.ServerCore.EventManager.StrongBind("habbo_login", EventPriority.After, SendToVirtualTestRoom);
        }

        private static Room GetVirtualTest(Habbo habbo)
        {
            if (_virtualTest == null)
            {
                _virtualTest = new RoomModelA(CoreManager.ServerCore.RoomDistributor.GetFreeRoomId())
                                   {
                                       Name = "VirtualTest1"
                                   };
            }
            return _virtualTest;
        }

        private static void SendToVirtualTestRoom(object source, EventArgs args)
        {
            Habbo habbo = (Habbo)source;
            habbo.SendToRoomManaged(GetVirtualTest(habbo));
        }
    }
}
