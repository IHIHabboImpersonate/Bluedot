using Bluedot.HabboServer.Rooms.Figure;
using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultHabboFunctions
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