namespace MessagePublisher.Shared.Models
{
    public class RecoveryInfo
    {
        public string Queue { get; private set; }
        public int StartSeqNumber { get; private set; }

        public RecoveryInfo(string queue, int startSeqNumber)
        {
            Queue = queue;
            StartSeqNumber = startSeqNumber;
        }
    }
}
