using System.Collections.Generic;

namespace MessagePublisher.Utility
{
    public class PoolIdGenerator
    {
        public static PoolIdGenerator Instance = new PoolIdGenerator();
        private HashSet<int> _existingPoolIds;
        private int _limit = 100000;

        private PoolIdGenerator()
        {
            _existingPoolIds = new HashSet<int>();
        }

        public int CreatePoolId()
        {
            lock (_existingPoolIds)
            {
                if (_existingPoolIds.Count >= _limit / 10)
                {
                    _limit += 100000;
                }
                while (true)
                {
                    int poolId = RandomGenerator.Instance.Next(_limit - 100000, _limit);
                    if (!_existingPoolIds.Contains(poolId))
                    {
                        _existingPoolIds.Add(poolId);
                        return poolId;
                    }
                }
            }
        }
    }
}
