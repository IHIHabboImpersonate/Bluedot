using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    public class MEventCategoryValid : OutgoingMessage
    {
        public int CategoryId { get; set; }

        public override OutgoingMessage Send(IMessageable target)
        {
            if (InternalOutgoingMessage.Id == 0)
            {
                InternalOutgoingMessage.Initialize(354).AppendInt32(CategoryId);
            }

            target.SendMessage(InternalOutgoingMessage);
            return this;
        }
    }
}