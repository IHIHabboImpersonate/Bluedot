using Bluedot.HabboServer.ApiUsage.Figures.Sets;
using Bluedot.HabboServer.Figures;

namespace Bluedot.HabboServer.ApiUsage.Figures
{
    public static class FigureRoot
    {
        public static void Start()
        {
            HabboFigureFactory factory = CoreManager.ServerCore.HabboFigureFactory;

            factory
                .RegisterSet(typeof(Body180));
        }
    }
}
