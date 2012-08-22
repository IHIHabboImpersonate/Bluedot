
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bluedot.HabboServer.Rooms
{
    public interface IRollerable
    {
        IRollerable Roll(FloorPosition to);
        IRollerable Roll(FloorPosition from, FloorPosition to);
    }
}
