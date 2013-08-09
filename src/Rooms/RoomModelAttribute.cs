using System;
using System.Collections.Generic;
using System.Drawing;

using Bluedot.HabboServer.ApiUsage.Libraries.Subscriptions;
using Bluedot.HabboServer.Database;
using Bluedot.HabboServer.Database.Actions;
using Bluedot.HabboServer.Useful;
using Bluedot.HabboServer.Rooms.Figure;
using Bluedot.HabboServer.Habbos.Messenger;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Permissions;

namespace Bluedot.HabboServer.Rooms
{
    public class RoomModelAttribute : Attribute
    {
        public string ModelName
        {
            get;
            private set;
        }

        public RoomModelAttribute(string modelName)
        {
            ModelName = modelName;
        }
    }
}