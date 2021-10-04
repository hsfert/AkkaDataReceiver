using Akka.Actor;
using DataReceiver.Shared.Messages;
using System.Collections.Generic;
using System.Linq;

namespace DataReceiver.Shared.Models
{
    public class PublisherDoneTask
    {
        public PublisherMessageMeta Meta { get; private set; }
        public IActorRef Sender { get; private set; }
        private HashSet<PoolMessageMeta> _poolsDone;
        private int _numberOfPools;
        public List<PoolMessageMeta> Pools { get { return _poolsDone.ToList(); } }

        public PublisherDoneTask(IActorRef sender, PublisherMessageMeta meta, int numberOfPools)
        {
            Sender = sender;
            Meta = meta;
            _numberOfPools = numberOfPools;
            _poolsDone = new HashSet<PoolMessageMeta>();
        }

        public bool DoTask(PoolMessageMeta pool)
        {
            _poolsDone.Add(pool);
            return _poolsDone.Count == _numberOfPools;
        }
    }
}
