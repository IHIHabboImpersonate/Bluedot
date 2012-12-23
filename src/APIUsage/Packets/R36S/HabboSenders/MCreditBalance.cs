using System.Globalization;

using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public class MCreditBalance : OutgoingMessage
    {
        public int Balance { get; set; }

        public override OutgoingMessage Send(IMessageable target)
        {
            if (InternalOutgoingMessage.Id == 0)
            {
                InternalOutgoingMessage.Initialize(6)
                    .AppendString(Balance.ToString(CultureInfo.InvariantCulture));
            }

            target.SendMessage(InternalOutgoingMessage);
            return this;
        }
    }
}