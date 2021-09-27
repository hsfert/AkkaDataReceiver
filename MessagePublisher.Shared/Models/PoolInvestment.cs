﻿using System.Collections.Generic;

namespace MessagePublisher.Shared.Models
{
    public class PoolInvestment
    {
        public long GameId { get; private set; }
        public int PoolId { get; private set; }
        public List<CombinationInvestment> Combinations { get; private set; }

        public PoolInvestment(long gameId, int poolId, List<CombinationInvestment> combinations)
        {
            GameId = gameId;
            PoolId = poolId;
            Combinations = combinations;
        }
    }
}
