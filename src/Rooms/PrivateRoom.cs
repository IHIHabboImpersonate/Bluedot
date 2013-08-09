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
    public abstract class PrivateRoom : Room
    {
        #region Properties
        #region Property: LockType
        /// <summary>
        /// 
        /// </summary>
        public string RoomLock
        {
            get;
            set;
        }
        #endregion

        #region Property: Owner
        /// <summary>
        /// 
        /// </summary>
        public IRoomOwner Owner
        {
            get;
            private set;
        }
        #endregion
        #endregion

        #region Method: PrivateRoom (Constructor)
        protected PrivateRoom(int id) : base(id) {}
        #endregion
    }
}