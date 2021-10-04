using System;

namespace DataReceiver.Shared.Messages
{
    public class PoolMessageMeta : IEquatable<PoolMessageMeta>
    {
        public long SeqNumber { get; private set; }
        public long PoolId { get; private set; }
        public PoolMessageType MessageType { get; private set; }

        public PoolMessageMeta(long seqNumber, long poolId, PoolMessageType messageType)
        {
            SeqNumber = seqNumber;
            PoolId = poolId;
            MessageType = messageType;
        }

        public bool Equals(PoolMessageMeta other)
        {
            return SeqNumber == other.SeqNumber && PoolId == other.PoolId && MessageType == other.MessageType;
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != this.GetType()) return false;
            return Equals((PoolMessageMeta)other);
        }

        public override int GetHashCode()
        {
            return (SeqNumber.GetHashCode() * 19 +
               PoolId.GetHashCode()) * 23 + MessageType.GetHashCode();
        }
    }
}
