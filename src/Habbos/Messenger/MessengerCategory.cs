using System;
using System.Collections.Generic;
using Bluedot.HabboServer.Useful;

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
        /// <summary>
        /// 
        /// </summary>
        public ResettableLazy<EventingCollection<HashSet<IBefriendable>, IBefriendable>> Friends
        {
            get;
            private set;
        }
        #endregion

        #region Method: MessengerCategory (Constructor)
        public MessengerCategory()
        {
            Friends = new ResettableLazy<EventingCollection<HashSet<IBefriendable>, IBefriendable>>(() => new EventingCollection<HashSet<IBefriendable>, IBefriendable>());
        }
        #endregion
    }
}
