using System;
using System.Collections.Generic;
using Bluedot.HabboServer.Habbos.Figure;

namespace Bluedot.HabboServer.Habbos.Messenger
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
        #region Property: Room
        // TODO: Add Rooms
        ///// <summary>
        ///// 
        ///// </summary>
        //Room Room
        //{
        //    get;
        //}
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

        #region Property: MessengerCategories
        /// <summary>
        /// 
        /// </summary>
        ICollection<MessengerCategory> MessengerCategories
        {
            get;
        }
        #endregion
    }
}
