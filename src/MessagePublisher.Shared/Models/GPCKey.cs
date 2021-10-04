using System;

namespace MessagePublisher.Shared.Models
{
    public class GPCKey : IEquatable<GPCKey>
    {
        public int GameId { get; private set; }
        public int PoolId { get; private set; }
        public short CombinationId { get; private set; }

        public GPCKey(int gameId, int poolId, short combinationId)
        {
            GameId = gameId;
            PoolId = poolId;
            CombinationId = combinationId;
        }

        public bool Equals(GPCKey other)
        {
            return GameId == other.GameId && PoolId == other.PoolId && CombinationId == other.CombinationId;
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != this.GetType()) return false;
            return Equals((GPCKey)other);
        }

        public override int GetHashCode()
        {
            return (GameId * 17 + PoolId) * 19 + CombinationId;
        }
    }
}
