using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultLoginFunctions
{
    public class MAuthenticationOkay : OutgoingMessage
    {
        public override OutgoingMessage Send(IMessageable target)
        {
            if (InternalOutgoingMessage.Id == 0)
            {
                InternalOutgoingMessage.Initialize(3);
            }
            target.SendMessage(InternalOutgoingMessage);
            return this;
        }
    }
}