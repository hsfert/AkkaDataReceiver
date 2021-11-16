using DataReceiverEntity = DataReceiver.Shared.Database.DataReceiverEntity;
using DbPool = DataReceiver.Shared.Database.Pool;
using DataReceiver.Shared.Database.ScriptHelper;
using System.Collections.Generic;
using System.Linq;

namespace DataReceiver.Shared.Models
{
    public class PoolDatabaseDictionary
    {
        private Dictionary<long, PoolManager> _pools;

        public PoolDatabaseDictionary()
        {
            _pools = new Dictionary<long, PoolManager>();
        }

        public PoolManager GetOrAddPool(int gameId, long poolId)
        {
            PoolManager pool;
            if (_pools.TryGetValue(poolId, out pool))
            {
                return pool;
            }
            GetPoolFromDatatbase(gameId, poolId);
            return _pools[poolId];
        }

        public List<PoolManager> GetAllPools()
        {
            return _pools.Values.ToList();
        }

        private void GetPoolFromDatatbase(int gameId, long poolId)
        {
            List<DbPool> inputs = new List<DbPool> { new DbPool
            {
                 id = poolId,
                 game_id = gameId,
                 instance_name = "1"
            } };

            List<DbPool> outputs;
            using (var content = new DataReceiverEntity())
            {
                var sql = PoolDatabaseHelper.Instance.ProduceSQL(inputs);
                outputs = content.SqlQuery<DbPool>(sql, null).ToList();
            }

            foreach(var output in outputs)
            {
                _pools[output.id] = new PoolManager(output.id, output.game_id);
            }
        }
    }
}
