namespace IHI.Server.Network
{
    public abstract class GameSocketReader
    {
        public abstract int LengthBytes { get; }
        public abstract int ParseLength(byte[] data);
        public abstract IncomingMessage ParseMessage(byte[] data);
    }
}