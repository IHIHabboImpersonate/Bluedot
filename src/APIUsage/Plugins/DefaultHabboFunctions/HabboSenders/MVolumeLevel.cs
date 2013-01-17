using Bluedot.HabboServer.Network;

namespace Bluedot.HabboServer.ApiUsage.Plugins.DefaultHabboFunctions
{
    public class MVolumeLevel : OutgoingMessage
    {
        private byte _volume;
        public byte Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                if (value > 100)
                    _volume = 100;
                else
                    _volume = value;
            }
        }

        public override OutgoingMessage Send(IMessageable target)
        {
            if (InternalOutgoingMessage.Id == 0)
            {
                InternalOutgoingMessage.Initialize(308).
                    AppendInt32(Volume);
            }

            target.SendMessage(InternalOutgoingMessage);
            return this;
        }
    }
}