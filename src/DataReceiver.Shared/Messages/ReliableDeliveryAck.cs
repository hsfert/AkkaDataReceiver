using System;

namespace DataReceiver.Shared.Messages
{
    public class ReliableDeliveryAck<Meta> where Meta : IEquatable<Meta>
    {
        public Meta MessageMeta { get; private set; }

        public ReliableDeliveryAck(Meta messageMeta)
        {
            MessageMeta = messageMeta;
        }
    }
}
