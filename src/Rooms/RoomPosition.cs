using System;
using System.Collections.Generic;
using System.Drawing;

using IHI.Server.Libraries.Subscriptions;
using IHI.Server.Database;
using IHI.Server.Database.Actions;
using IHI.Server.Useful;
using IHI.Server.Rooms.Figure;
using IHI.Server.Habbos.Messenger;
using IHI.Server.Network;
using IHI.Server.Permissions;

namespace IHI.Server.Rooms
{
    public struct RoomPosition
    {
        #region Properties
        #region Property: HotelView
        private static readonly RoomPosition _hotelView = new RoomPosition();
        public static RoomPosition HotelView
        {
            get
            {
                return _hotelView;
            }
        }
        #endregion

        #region Property: Room
        private readonly Room _room;
        public Room Room
        {
            get
            {
                return _room;
            }
        }
        #endregion
        #region Property: X
        private readonly int _x;
        public int X
        {
            get
            {
                return _x;
            }
        }

        #endregion
        #region Property: Y
        private readonly int _y;
        public int Y
        {
            get
            {
                return _y;
            }
        }
        #endregion
        #region Property: Z
        private readonly float _z;
        public float Z
        {
            get
            {
                return _z;
            }
        }
        #endregion
        #region Property: Rotation
        private readonly byte _rotation;
        public byte Rotation
        {
            get
            {
                return _rotation;
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Method: RoomPosition (Constructor)
        public RoomPosition(int x, int y, float z, byte rotation, Room room = null)
        {
            _x = x;
            _y = y;
            _z = z;
            _rotation = rotation;
            _room = room;
        }
        public RoomPosition(RoomPosition template, int x, int y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
            _rotation = template.Rotation;
            _room = template.Room;
        }
        public RoomPosition(RoomPosition template, int x, int y, float z, byte rotation)
        {
            _x = x;
            _y = y;
            _z = z;
            _rotation = rotation;
            _room = template.Room;
        }
        public RoomPosition(RoomPosition template, Room room = null)
        {
            _x = template.X;
            _y = template.Y;
            _z = template.Z;
            _rotation = template.Rotation;
            _room = room;
        }
        #endregion
        #endregion
    }
}