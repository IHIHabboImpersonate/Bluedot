using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Rooms.Navigator;

namespace Bluedot.HabboServer.Rooms
{
    public abstract class Room : NavigatorListing, IMessageable
    {
        protected ICollection<RoomUnit> _occupants;
        private ICollection<Category> _secondaryCategories;

        public Habbo Owner { get; set; }
        public RoomLock LockMode { get; set; }

        /// <summary>
        ///   The description of the room.
        /// </summary>
        public string Description { get; set; }

        protected Room()
        {
            _secondaryCategories = new HashSet<Category>();
        }

        public IMessageable SendMessage(IInternalOutgoingMessage message)
        {
            throw new NotImplementedException();
        }

        public Room AddSecondaryCategory(Category category)
        {
            if (_secondaryCategories.Contains(category))
                return this;
            _secondaryCategories.Add(category);
            category.AddListing(this);
            return this;
        }

        public Room RemoveSecondaryCategory(Category category)
        {
            if (!_secondaryCategories.Contains(category))
                return this;
            category.RemoveListing(this);
            _secondaryCategories.Remove(category);
            return this;
        }
    }
}
