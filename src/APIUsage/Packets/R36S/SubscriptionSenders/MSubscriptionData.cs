using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public class MSubscriptionData : OutgoingMessage
    {
        public string SubscriptionName { get; set; }
        public int CurrentDay { get; set; }
        public int ElapsedMonths { get; set; }
        public int PrepaidMonths { get; set; }
        public bool IsActive { get; set; }

        public override OutgoingMessage Send(IMessageable target)
        {
            if (InternalOutgoingMessage.Id == 0)
            {
                InternalOutgoingMessage.Initialize(7)
                    .AppendString(SubscriptionName)
                    .AppendInt32(CurrentDay)
                    .AppendInt32(ElapsedMonths)
                    .AppendInt32(PrepaidMonths)
                    .AppendBoolean(IsActive);
            }

            target.SendMessage(InternalOutgoingMessage);
            return this;
        }
    }
}