using DbCombination = DataReceiver.Shared.Database.Combination;
using DataReceiverEntity = DataReceiver.Shared.Database.DataReceiverEntity;
using DataReceiver.Shared.Database.ScriptHelper;
using MessagePublisher.Shared.Models;
using System.Collections.Generic;
using System.Linq;

namespace DataReceiver.Shared.Models
{
    public class CombinationDatabaseDictionary
    {
        private int _gameId;
        private Dictionary<GPCKey, CombinationManager> _combinations;

        public CombinationDatabaseDictionary(int gameId)
        {
            _gameId = gameId;
            _combinations = new Dictionary<GPCKey, CombinationManager>();
        }

        public Dictionary<GPCKey, CombinationManager> GetOrAddCombinations(List<GPCKey> keys)
        {
            Dictionary<GPCKey, CombinationManager> output = new Dictionary<GPCKey, CombinationManager>();
            List<GPCKey> keysToBeAdded = new List<GPCKey>();
            foreach (var key in keys)
            {
                CombinationManager combination;
                if (_combinations.TryGetValue(key, out combination))
                {
                    output[key] = combination;
                }
                else
                {
                    keysToBeAdded.Add(key);
                }
            }
            if (keysToBeAdded.Count > 0)
            {
                GetCombinationsFromDatatbase(keysToBeAdded);
                foreach (var key in keysToBeAdded)
                {
                    output[key] = _combinations[key];
                }
            }
            return output;
        }

        public List<CombinationManager> GetAllCombinations()
        {
            return _combinations.Values.ToList();
        }

        private void GetCombinationsFromDatatbase(List<GPCKey> keys)
        {
            List<DbCombination> inputs = keys.Select(key => new DbCombination
            {
                pool_id = key.PoolId,
                combination_id = key.CombinationId
            }).ToList();

            List<DbCombination> outputs;
            using (var content = new DataReceiverEntity())
            {
                var sql = CombinationCreationHelper.Instance.ProduceSQL(inputs);
                outputs = content.SqlQuery<DbCombination>(sql, null).ToList();
            }

            foreach (var output in outputs)
            {
                var key = new GPCKey(_gameId, output.pool_id, output.combination_id);
                var combination = new CombinationManager(key, output);
                _combinations[key] = combination;
            }
        }
    }
}
