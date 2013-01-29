using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Rooms;

namespace Bluedot.HabboServer.APIUsage.Plugins.ManagedRoomLoading
{
    public static class ExtensionMethods
    {
        #region Methods
        #region Method: Habbo.GetRoomManaged
        public static Room GetRoomManaged(this Habbo habbo)
        {
            byte[] roomIdBytes = habbo.InstanceStorage["managed_room_loading:current_room"];
            if (roomIdBytes == null)
                return null;

            int i = 0;
            byte[] a = new byte[4];

            a[0] = (byte)(i >> 030);
            a[1] = (byte)(i >> 020);
            a[2] = (byte)(i >> 010);
            a[3] = (byte)i;

            int trueRoomId = roomIdBytes[0];
            trueRoomId = (trueRoomId << 010) | roomIdBytes[1];
            trueRoomId = (trueRoomId << 010) | roomIdBytes[2];
            trueRoomId = (trueRoomId << 010) | roomIdBytes[3];

            return CoreManager.ServerCore.RoomDistributor.GetRoom(trueRoomId, habbo);
        }
        #endregion
        #region Method: Habbo.SetRoomManaged
        private static Habbo SetRoomManaged(this Habbo habbo, Room room)
        {
            if (room == null)
                habbo.InstanceStorage["managed_room_loading:current_room"] = null;
            else
            {
                byte[] roomIdBytes = new byte[4];
                roomIdBytes[0] = (byte)(room.Id >> 030);
                roomIdBytes[1] = (byte)(room.Id >> 020);
                roomIdBytes[2] = (byte)(room.Id >> 010);
                roomIdBytes[3] = (byte)room.Id;
                habbo.InstanceStorage["managed_room_loading:current_room"] = roomIdBytes;
            }

            return habbo;
        }
        #endregion

        #region Method: Habbo.SendToRoomManaged
        public static void SendToRoomManaged(this Habbo habbo, Room room)
        {
            habbo.SendToRoomManaged(room, room.Door);
        }
        public static void SendToRoomManaged(this Habbo habbo, Room room, RoomPosition spawnPosition)
        {
            RoomPosition currentPosition = habbo.Position;
            if (room == null)
            {
                habbo.SetRoomManaged(null);
                if (currentPosition.Room != null)
                    currentPosition.Room.RemoveRoomUnit(habbo);
                habbo.Position = RoomPosition.HotelView;

                // TODO: Send to hotel view (packet wise).
            }
            else
            {
                if (currentPosition.Room != room)
                {
                    if(currentPosition.Room != null)
                        currentPosition.Room.RemoveRoomUnit(habbo);
                    room.AddRoomUnit(habbo);

                    habbo.SetRoomManaged(room);
                    habbo.Position = new RoomPosition(spawnPosition, room);
                }
                // TODO: Send the packet to force a reload.
            }
        }
        #endregion
        #endregion
    }
}
