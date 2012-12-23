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

using Bluedot.HabboServer.Habbos.Figure;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Packets
{
    using System.Globalization;

    public class MHabboData : OutgoingMessage
    {
        public HabboFigure Figure { get; set; }
        public int HabboId { get; set; }
        public string Motto { get; set; }
        public string UnknownA { get; set; }
        public string Username { get; set; }

        public override OutgoingMessage Send(IMessageable target)
        {
            if (InternalOutgoingMessage.Id == 0)
            {
                InternalOutgoingMessage.Initialize(5)
                    .AppendString(HabboId.ToString(CultureInfo.InvariantCulture))
                    .AppendString(Username) // TODO: Should this be display name?
                    .AppendString(Figure.ToString())
                    .AppendString(Figure.GenderChar.ToString(CultureInfo.InvariantCulture))
                    .AppendString(Motto)
                    .AppendString(UnknownA)
                    .AppendInt32(12) // TODO: Find out what this does.
                    .AppendString(Figure.FormattedSwimFigure)
                    .AppendBoolean(false)
                    .AppendBoolean(true);
            }

            target.SendMessage(InternalOutgoingMessage);
            return this;
        }
    }
}