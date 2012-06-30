using System.Collections.Generic;

namespace Bluedot.HabboServer.Habbos.Messenger
{
    public class MessengerCategory
    {
        #region Property: ID
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            get;
            private set;
        }
        #endregion
        #region Property: Name
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        #endregion
        #region Property: Friends
        private HashSet<IBefriendable> _friends;
        /// <summary>
        /// 
        /// </summary>
        public HashSet<IBefriendable> Friends
        {
            get
            {
                return _friends;
            }
            set
            {
                _friends = value;
            }
        }
        #endregion
    }
}
