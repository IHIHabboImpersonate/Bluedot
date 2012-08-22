using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bluedot.HabboServer.Rooms
{
    public abstract class Human : RoomUnit
    {
        public virtual bool IsWaving
        {
            get;
            set;
        }
        public virtual DanceType DanceType
        {
            get;
            set;
        }
    }
}
