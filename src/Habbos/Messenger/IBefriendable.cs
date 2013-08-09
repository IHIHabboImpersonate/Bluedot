using System;
using IHI.Server.Rooms;
using IHI.Server.Rooms.Figure;

namespace IHI.Server.Habbos.Messenger
{
    public interface IBefriendable
    {
        #region Property: Id
        /// <summary>
        /// 
        /// </summary>
        int Id
        {
            get;
        }
        #endregion
        #region Property: DisplayName
        /// <summary>
        /// 
        /// </summary>
        string DisplayName
        {
            get;
        }
        #endregion
        #region Property: Motto
        /// <summary>
        /// 
        /// </summary>
        string Motto
        {
            get;
        }
        #endregion
        #region Property: Figure
        /// <summary>
        /// 
        /// </summary>
        HabboFigure Figure
        {
            get;
        }
        #endregion
        #region Property: Position
        /// <summary>
        /// 
        /// </summary>
        RoomPosition Position
        {
            get;
        }
        #endregion

        #region Property: LoggedIn
        /// <summary>
        /// 
        /// </summary>
        bool LoggedIn
        {
            get;
        }
        #endregion
        #region Property: LastAccess
        /// <summary>
        /// 
        /// </summary>
        DateTime LastAccess
        {
            get;
        }
        #endregion

        #region Blocking Properties
        #region Property: Stalkable
        /// <summary>
        /// 
        /// </summary>
        bool Stalkable
        {
            get;
        }
        #endregion
        #region Property: Requestable
        /// <summary>
        /// 
        /// </summary>
        bool Requestable
        {
            get;
        }
        #endregion
        #region Property: Inviteable
        /// <summary>
        /// 
        /// </summary>
        bool Inviteable
        {
            get;
        }
        #endregion
        #endregion
    }
}
