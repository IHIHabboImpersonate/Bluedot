using Bluedot.HabboServer.Habbos;

namespace Bluedot.HabboServer.ApiUsage
{
    public static class Extensions
    {
        #region Type: Bluedot.HabboServer.Habbo
        #region Property: Volume
        public static byte VolumeProperty(this Habbo habbo)
        {
            if (habbo.PersistentStorage["Client.Volume"] == null)
                return 100;
            return habbo.PersistentStorage["Client.Volume"][0];
        }
        public static Habbo VolumeProperty(this Habbo habbo, byte volume)
        {
            if (volume > 100)
                volume = 100;
            habbo.PersistentStorage["Client.Volume"] = new[] {volume};

            return habbo;
        }
        #endregion
        #endregion
    }
}
