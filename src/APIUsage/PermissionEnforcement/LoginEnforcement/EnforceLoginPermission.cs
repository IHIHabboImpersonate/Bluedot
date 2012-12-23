using System;
using System.ComponentModel;

using Bluedot.HabboServer.Habbos;

namespace Bluedot.HabboServer.ApiUsage.PermissionEnforcement
{
    public static partial class LoginEnforcement
    {
        internal static void EnforceLoginPermission(object sender, EventArgs eventArgs)
        {
            Habbo habbo = (Habbo)sender;

            if (!habbo.HasPermission("habbo_login"))
                ((CancelEventArgs)eventArgs).Cancel = true;
        }
    }
}
