using System;
using System.Collections.Generic;
using System.Drawing;

using Bluedot.HabboServer.ApiUsage.Libraries.Subscriptions;
using Bluedot.HabboServer.Database;
using Bluedot.HabboServer.Database.Actions;
using Bluedot.HabboServer.Useful;
using Bluedot.HabboServer.Rooms.Figure;
using Bluedot.HabboServer.Habbos.Messenger;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Permissions;

namespace Bluedot.HabboServer.Rooms
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