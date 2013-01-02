using System.Collections.Generic;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.Habbos.Messenger
{
    using System.Linq;

    public class MessengerCategory
    {
        #region Field: _habboFriends
        /// <summary>
        /// Friends who are actually Habbos.
        /// </summary>
        private readonly BluedotDictionary<int, Habbo> _habboFriends;
        #endregion

        #region Field: _otherFriends
        /// <summary>
        /// Friends who are not actually Habbos.
        /// </summary>
        private readonly HashSet<IBefriendable> _otherFriends;
        #endregion

        #region Property: Id
        /// <summary>
        /// 
        /// </summary>
        public int Id
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
        public IEnumerable<IBefriendable> Friends
        {
            get
            {
                return _habboFriends.Values.Union(_otherFriends);
            }
        }
        #endregion

        private static readonly BluedotDictionary<int, Habbo>.LazyLoadingBehaviour _habboLazyLoadingBehavour = new BluedotDictionary<int, Habbo>.LazyLoadingBehaviour(true, id => CoreManager.ServerCore.HabboDistributor[id]);
        private static readonly BluedotDictionary<int, Habbo>.WeakReferenceBehaviour _habboWeakReferenceBehaviour = new BluedotDictionary<int, Habbo>.WeakReferenceBehaviour(true);

        #region Method: MessengerCategory (Constructor)
        public MessengerCategory()
        {
            _habboFriends = new BluedotDictionary<int, Habbo>(_habboLazyLoadingBehavour, weakReference: _habboWeakReferenceBehaviour);
            _otherFriends = new HashSet<IBefriendable>();
        }
        #endregion

        #region Method: AddFriend
        public MessengerCategory AddFriend(IBefriendable friend)
        {
            if (friend is Habbo)
                _habboFriends.Add(friend.Id, friend as Habbo);
            else
                _otherFriends.Add(friend);
            return this;
        }
        #endregion
    }
}
