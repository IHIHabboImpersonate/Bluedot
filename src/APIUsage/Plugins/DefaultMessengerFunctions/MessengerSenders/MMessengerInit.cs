using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bluedot.HabboServer.Habbos.Messenger;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultMessengerFunctions
{
    public class MMessengerInit : OutgoingMessage
    {
        public IEnumerable<MessengerCategory> Categories { get; set; }
        public int UnknownA { get; set; }
        public int UnknownB { get; set; }
        public int UnknownC { get; set; }
        public int MaximumFriends { get; set; }

        public override OutgoingMessage Send(IMessageable target)
        {
            if (InternalOutgoingMessage.Id == 0)
            {
                InternalOutgoingMessage.Initialize(12)
                    .AppendInt32(UnknownA)
                    .AppendInt32(UnknownB)
                    .AppendInt32(UnknownC)
                    .AppendInt32(Categories.Count()); // -1 because the default category doesn't count

                int friendCount = 0;

                foreach (MessengerCategory category in Categories)
                {
                    InternalOutgoingMessage
                        .AppendInt32(category.Id)
                        .AppendString(category.Name);
                    friendCount += category.Friends.Count();
                }

                InternalOutgoingMessage.AppendInt32(friendCount);

                foreach (MessengerCategory category in Categories)
                {
                    foreach (IBefriendable befriendable in category.Friends)
                    {
                        InternalOutgoingMessage
                            .AppendInt32(befriendable.Id)
                            .AppendString(befriendable.DisplayName)
                            .AppendBoolean(true) // TODO: Find out what this does.
                            .AppendBoolean(befriendable.LoggedIn)
                            .AppendBoolean(befriendable.Stalkable)
                            .AppendString(befriendable.Figure.ToString())
                            .AppendInt32(category.Id)
                            .AppendString(befriendable.Motto)
                            .AppendString(befriendable.LastAccess.ToString(CultureInfo.InvariantCulture));
                    }
                }

                InternalOutgoingMessage
                    .AppendInt32(MaximumFriends)
                    .AppendBoolean(false); // TODO: Find out what this does.
            }

            target.SendMessage(InternalOutgoingMessage);
            return this;
        }
    }
}