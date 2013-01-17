using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultLoginFunctions
{
    public enum ConnectionClosedReason
    {
        InvalidSSOTicket = 22,
        ConcurrentLogin = -1
    }

    public class MConnectionClosed : OutgoingMessage
    {
        public ConnectionClosedReason Reason { get; set; }

        public override OutgoingMessage Send(IMessageable target)
        {
            if (InternalOutgoingMessage.Id == 0)
            {
                InternalOutgoingMessage.Initialize(287)
                    .AppendInt32((int)Reason);
            }

            target.SendMessage(InternalOutgoingMessage);
            return this;
        }
    }
}