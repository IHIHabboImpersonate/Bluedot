using System;

using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Habbos.Figure;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.ClassicFigures
{
    public class ClassicFigures : ITempPlugin
    {
        public void Start()
        {
            HabboFigureFactory factory = CoreManager.ServerCore.HabboFigureFactory;

            factory
                .RegisterSet(typeof(Body180));
        }
    }
}
