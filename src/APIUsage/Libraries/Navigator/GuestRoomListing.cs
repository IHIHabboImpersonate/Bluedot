using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bluedot.HabboServer.Events;
using Bluedot.HabboServer.Habbos;

namespace Bluedot.HabboServer.APIUsage.Libraries.Navigator
{
    public abstract class GuestRoomListing : RoomListing
    {
        #region Properties

        public Habbo Owner { get; set; }
        public RoomLock LockMode { get; set; }

        #endregion
    }
}
