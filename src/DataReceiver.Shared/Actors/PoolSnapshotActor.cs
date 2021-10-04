using Akka.Actor;
using DataReceiver.Shared.Messages;
using DataReceiver.Shared.Models;
using System;
using System.Collections.Generic;

namespace DataReceiver.Shared.Actors
{
    public class PoolSnapshotActor : ReceiveActor
    {
        private Dictionary<long, Pool> _pools;
        public PoolSnapshotActor()
        {
            _pools = new Dictionary<long, Pool>();
            Become(UpdatePool);
        }

        private void UpdatePool()
        {
            Receive<IPoolMessage>(message =>
            {
                Console.WriteLine($"{DateTime.Now} Received message with SeqNumber {message.SeqNumber}, PoolId {message.PoolId} and MessageType {message.MessageType}");
                Pool pool;
                if(!_pools.TryGetValue(message.PoolId, out pool))
                {
                    pool = new Pool(message.PoolId, message.GameId);
                    _pools[message.PoolId] = pool;
                }
                if(message is PoolInvestmentUpdateMessage investment)
                {
                    pool.UpdateInvestment(investment.SeqNumber, investment.Combinations);
                }
                else if(message is PoolOddsUpdateMessage oddsChange)
                {
                    pool.UpdateOdds(oddsChange.SeqNumber, oddsChange.Combinations);
                }
                var header = new PoolMessageMeta(message.SeqNumber, message.PoolId, message.MessageType);
                Sender.Tell(new ReliableDeliveryAck<PoolMessageMeta>(header));
            });
        }

        protected override void PreStart()
        {
            Console.WriteLine("Setting up PoolSnapshotActor with Path " + Self.Path);
            base.PreStart();
        }

        protected override void PostStop()
        {
            Console.WriteLine("Destorying PoolSnapshotActor with Path " + Self.Path);
            base.PostStop();
        }
    }
}
