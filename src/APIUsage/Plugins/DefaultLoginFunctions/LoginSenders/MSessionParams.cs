using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultLoginFunctions
{
    public class MSessionParams : OutgoingMessage
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int D { get; set; }
        public string DateFormat { get; set; }
        public int E { get; set; }
        public int F { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public int I { get; set; }
        public int J { get; set; }
        public int K { get; set; }
        public int L { get; set; }
        public string M { get; set; }
        public int N { get; set; }
        public bool O { get; set; }
        public int P { get; set; }
        public string Q { get; set; }
        public int R { get; set; }
        public bool S { get; set; }
        public string URL { get; set; }

        public override OutgoingMessage Send(IMessageable target)
        {
            if (InternalOutgoingMessage.Id == 0)
            {
                InternalOutgoingMessage.Initialize(257)
                    .AppendInt32(A)
                    .AppendInt32(B)
                    .AppendInt32(C)
                    .AppendInt32(D)
                    .AppendInt32(E)
                    .AppendInt32(F)
                    .AppendInt32(G)
                    .AppendInt32(H)
                    .AppendInt32(I)
                    .AppendInt32(J)
                    .AppendInt32(K)
                    .AppendInt32(L)
                    .AppendString(DateFormat)
                    .AppendString(M)
                    .AppendInt32(N)
                    .AppendBoolean(O)
                    .AppendInt32(P)
                    .AppendString(URL)
                    .AppendString(Q)
                    .AppendInt32(R)
                    .AppendBoolean(S);
            }

            target.SendMessage(InternalOutgoingMessage);
            return this;
        }
    }
}