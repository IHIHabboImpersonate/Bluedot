using System;

using IHI.Server.Habbos;
using IHI.Server.Rooms.Figure;
using IHI.Server.Network;

namespace IHI.Server.Plugins.ClassicFigures
{
    public class ClassicFigures : Plugin
    {
        public override void Start(EventFirer eventFirer)
        {
            HabboFigureFactory factory = CoreManager.ServerCore.HabboFigureFactory;

            factory
                .RegisterSet(typeof(Body180));
        }
    }
}
