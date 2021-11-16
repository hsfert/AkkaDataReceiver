using Akka.Actor;
using DbCombination = DataReceiver.Shared.Database.Combination;
using DataReceiverEntity = DataReceiver.Shared.Database.DataReceiverEntity;
using DataReceiver.Shared.Database.ScriptHelper;
using DataReceiver.Shared.Messages;
using DataReceiver.Shared.Models;
using System;
using System.Collections.Generic;

namespace DataReceiver.Shared.Actors
{
    public class PoolSnapshotActor : ReceiveActor
    {
        private ICancelable _recurringDatabaseUpdate;
        private PoolDatabaseDictionary _pools;
        public PoolSnapshotActor()
        {
            _pools = new PoolDatabaseDictionary();
            Become(UpdatePool);
        }

        private void UpdatePool()
        {
            Receive<UpdateDatabase>(_ =>
            {
                FlushDatabaseUpdate();
            });

            Receive<IPoolMessage>(message =>
            {
                Console.WriteLine($"{DateTime.Now} Received message with SeqNumber {message.SeqNumber}, PoolId {message.PoolId} and MessageType {message.MessageType}");
                PoolManager pool = _pools.GetOrAddPool(message.GameId, message.PoolId);
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

        private void FlushDatabaseUpdate()
        {
            List<DbCombination> updatedEntries = new List<DbCombination>();
            foreach (var pool in _pools.GetAllPools())
            {
                List<DbCombination> updatedEntriesInPool;
                if (pool.GetUpdatedEntries(out updatedEntriesInPool))
                {
                    updatedEntries.AddRange(updatedEntriesInPool);
                }
            }

            if (updatedEntries.Count > 0)
            {
                using (var content = new DataReceiverEntity())
                {
                    string sql = CombinationUpdateHelper.Instance.ProduceSQL(updatedEntries);
                    content.ExecuteSqlCommand(sql, null);
                }
            }
        }

        protected override void PreStart()
        {
            _recurringDatabaseUpdate =
              Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(10),
              TimeSpan.FromSeconds(10), Self, UpdateDatabase.Instance, ActorRefs.NoSender);
            base.PreStart();
        }

        protected override void PostStop()
        {
            Console.WriteLine("Destorying PoolSnapshotActor with Path " + Self.Path);
            _recurringDatabaseUpdate?.Cancel();
            Console.WriteLine("Trying to flush the updates...");
            FlushDatabaseUpdate();
            base.PostStop();
        }
    }
}
