using System;
using System.Collections.Generic;

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