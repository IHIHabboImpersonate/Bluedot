using Bluedot.HabboServer.Habbos;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultLoginFunctions
{
    internal static partial class PacketHandlers
    {
        internal static void ProcessSessionRequest(Habbo sender, IncomingMessage message)
        {
            new MSessionParams
            {
                A = 9,
                B = 0,
                C = 0,
                D = 1,
                E = 1,
                F = 3,
                G = 0,
                H = 2,
                I = 1,
                J = 4,
                K = 0,
                L = 5,
                DateFormat = "dd-MM-yyyy",
                M = "",
                N = 7,
                O = false,
                P = 8,
                URL = "http://ihi.cecer1.com",
                Q = "",
                R = 9,
                S = false
            }.Send(sender);
        }
    }
}
