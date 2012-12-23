#region GPLv3

// 
// Copyright (C) 2012  Chris Chenery
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bluedot.HabboServer.Habbos.Messenger;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
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