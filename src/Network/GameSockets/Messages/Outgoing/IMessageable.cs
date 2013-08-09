using System;

namespace IHI.Server.Network
{
    public delegate void GameSocketMessageEvent(object sender, GameSocketMessageEventArgs args);

    public class GameSocketMessageEventArgs : EventArgs
    {
        public IInternalOutgoingMessage Message
        {
            get;
            private set;
        }
        public GameSocketMessageEventArgs(IInternalOutgoingMessage message)
        {
            Message = message;
        }
    }

    public interface IMessageable
    {
        IMessageable SendMessage(IInternalOutgoingMessage message);
    }
}