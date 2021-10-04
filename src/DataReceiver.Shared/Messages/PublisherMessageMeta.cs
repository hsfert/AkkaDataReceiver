using System;

namespace DataReceiver.Shared.Messages
{
    public class PublisherMessageMeta : IEquatable<PublisherMessageMeta>
    {
        public long SeqNumber { get; private set; }
        public string Queue { get; private set; }

        public PublisherMessageMeta(long seqNumber, string queue)
        {
            SeqNumber = seqNumber;
            Queue = queue;
        }

        public bool Equals(PublisherMessageMeta other)
        {
            return SeqNumber == other.SeqNumber && Queue == other.Queue;
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != this.GetType()) return false;
            return Equals((PublisherMessageMeta)other);
        }

        public override int GetHashCode()
        {
            return SeqNumber.GetHashCode() * 19 +
               (Queue != null ? Queue.GetHashCode() : 0);
        }
    }
}
