using System;
using System.Collections.Generic;

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
    public interface IRoomUnit
    {
        #region Properties
        #region Property: RoomUnitId
        int RoomUnitId
        {
            get;
            set;
        }
        #endregion

        #region Property: DisplayName
        /// <summary>
        /// The display name of this RoomUnit.
        /// </summary>
        string DisplayName
        {
            get;
            set;
        }
        #endregion

        #region Property: Position
        RoomPosition Position
        {
            get;
        }
        #endregion

        #region Property: GenericFigure
        RoomUnitFigure GenericFigure
        {
            get;
        }
        #endregion
        #endregion
    }
}