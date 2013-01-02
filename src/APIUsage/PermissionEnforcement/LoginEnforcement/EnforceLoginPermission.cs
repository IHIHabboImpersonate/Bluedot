using System;

using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Useful;

namespace Bluedot.HabboServer.ApiUsage.PermissionEnforcement
{
    public static partial class LoginEnforcement
    {
        internal static void EnforceLoginPermission(object sender, EventArgs eventArgs)
        {
            Habbo habbo = (Habbo)sender;
            CancelReasonEventArgs cancelReasonEventArgs = (CancelReasonEventArgs)eventArgs;
            if (!habbo.HasPermission("habbo_login"))
            {
                cancelReasonEventArgs.Cancel = true;
                cancelReasonEventArgs.CancelReason = "Permission \"habbo_login\" missing!";
            }
        }
    }
}
