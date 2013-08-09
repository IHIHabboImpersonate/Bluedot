#region Usings

using IHI.Server.Habbos;

#endregion

namespace IHI.Server.Network
{
    public delegate void GameSocketMessageHandler(Habbo sender, IncomingMessage message);

    public class GameSocketMessageHandlers
    {
        public GameSocketMessageHandler HighPriority;
        public GameSocketMessageHandler LowPriority;
        public GameSocketMessageHandler DefaultAction;
        public GameSocketMessageHandler Watcher;

        public GameSocketMessageHandlers Invoke(Habbo sender, IncomingMessage message)
        {
            if (HighPriority != null)
                HighPriority.Invoke(sender, message);

            if (message.Cancelled)
                return this;

            if (LowPriority != null)
                LowPriority.Invoke(sender, message);

            if (message.Cancelled)
                return this;

            if (DefaultAction != null)
                DefaultAction.Invoke(sender, message);

            if (Watcher != null)
                Watcher.Invoke(sender, message);

            return this;
        }
    }
}