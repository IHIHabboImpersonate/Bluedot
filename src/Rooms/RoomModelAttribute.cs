using System;
using System.Collections.Generic;
using System.Drawing;

using IHI.Server.Libraries.Subscriptions;
using IHI.Server.Database;
using IHI.Server.Database.Actions;
using IHI.Server.Useful;
using IHI.Server.Rooms.Figure;
using IHI.Server.Habbos.Messenger;
using IHI.Server.Network;
using IHI.Server.Permissions;

namespace IHI.Server.Rooms
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
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