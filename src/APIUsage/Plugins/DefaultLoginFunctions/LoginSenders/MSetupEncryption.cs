using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultLoginFunctions
{
    public class MSetupEncryption : OutgoingMessage
    {
        public bool UnknownA { get; set; }

        public override OutgoingMessage Send(IMessageable target)
        {
            if (InternalOutgoingMessage.Id == 0)
            {
                InternalOutgoingMessage.Initialize(277)
                    .AppendBoolean(UnknownA);
            }
            target.SendMessage(InternalOutgoingMessage);
            return this;
        }
    }
}