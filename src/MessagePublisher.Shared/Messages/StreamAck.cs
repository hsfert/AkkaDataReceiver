namespace MessagePublisher.Shared.Messages
{
    public class StreamAck
    {
        public static StreamAck Instance = new StreamAck();
        private StreamAck()
        {

        }
    }
}
