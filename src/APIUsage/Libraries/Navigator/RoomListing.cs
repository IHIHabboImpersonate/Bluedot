using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bluedot.HabboServer.Events;

namespace Bluedot.HabboServer.APIUsage.Libraries.Navigator
{
    public abstract class RoomListing : Listing
    {
        #region Fields

        private ICollection<Category> _secondaryCategories;

        #endregion

        #region Properties

        /// <summary>
        ///   The description of the room.
        /// </summary>
        public string Description { get; set; }

        #endregion
    }
}
