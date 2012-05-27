using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bluedot.HabboServer.Habbos;

namespace Bluedot.HabboServer.APIUsage
{
    public static class Extensions
    {
        #region Type: Bluedot.HabboServer.Habbo
        #region Property: Volume
        public static byte VolumeProperty(this Habbo habbo)
        {
            if (habbo.PersistentValues["Client.Volume"] == null)
                return 100;
            return habbo.PersistentValues["Client.Volume"][0];
        }
        public static Habbo VolumeProperty(this Habbo habbo, byte volume)
        {
            if (volume > 100)
                volume = 100;
            habbo.PersistentValues["Client.Volume"] = new[] {volume};

            return habbo;
        }
        #endregion
        #endregion
    }
}
