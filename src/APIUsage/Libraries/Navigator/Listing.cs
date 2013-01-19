using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bluedot.HabboServer.Events;

namespace Bluedot.HabboServer.APIUsage.Libraries.Navigator
{
    public abstract class Listing
    {
        #region Fields

        private Category _primaryCategory;
        // TODO: Per-listing listing showing

        #endregion

        #region Properties
        /// <summary>
        /// The ID that will be sent to the client.
        /// </summary>
        public int Id { get; set; }

        public Category PrimaryCategory
        {
            set
            {
                if (PrimaryCategory == value)
                    return;

                EventManager events = CoreManager.ServerCore.EventManager;

                if (_primaryCategory != null && _primaryCategory.RemoveListing(this))
                    _primaryCategory = null;

                if (value != null && value.AddListing(this))
                    _primaryCategory = value;
            }
            get { return _primaryCategory; }
        }

        /// <summary>
        ///   The name of the room.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///   The current population of the room.
        /// </summary>
        public virtual int Population
        {
            get;
            set;
        }

        /// <summary>
        ///   The maximum population of the room.
        /// </summary>
        public virtual int Capacity
        {
            get;
            set;
        }
        #endregion
    }
}
