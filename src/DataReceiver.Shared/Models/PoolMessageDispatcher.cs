using DataReceiver.Shared.Messages;
using MessagePublisher.Shared.Messages;
using System.Collections.Generic;

namespace DataReceiver.Shared.Models
{
    public class PoolMessageDispatcher
    {
        public List<IPoolMessage> RedistributeMessage(IPublisherMessage message)
        {
            if (message is InvestmentSnapshotMessage investment)
            {
                return RedistributeMessage(investment);
            }
            if (message is OddsChangeMessage oddsChange)
            {
                return RedistributeMessage(oddsChange);
            }
            return new List<IPoolMessage>();
        }

        private List<IPoolMessage> RedistributeMessage(InvestmentSnapshotMessage message)
        {
            List<IPoolMessage> outputs = new List<IPoolMessage>();
            foreach (var pool in message.Investments)
            {
                PoolInvestmentUpdateMessage output = new PoolInvestmentUpdateMessage(message.SeqNumber,
                    pool.GameId, pool.PoolId, pool.Combinations);
                outputs.Add(output);
            }
            return outputs;
        }

        private List<IPoolMessage> RedistributeMessage(OddsChangeMessage message)
        {
            List<IPoolMessage> outputs = new List<IPoolMessage>();
            foreach (var pool in message.OddsChange)
            {
                PoolOddsUpdateMessage output = new PoolOddsUpdateMessage(message.SeqNumber,
                    pool.GameId, pool.PoolId, pool.Combinations);
                outputs.Add(output);
            }
            return outputs;
        }
    }
}
