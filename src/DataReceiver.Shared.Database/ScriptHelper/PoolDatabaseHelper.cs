using SQLScriptHelper;

namespace DataReceiver.Shared.Database.ScriptHelper
{
    public class PoolDatabaseHelper : MergeScriptHelper<Pool>
    {
        public static PoolDatabaseHelper Instance = new PoolDatabaseHelper();
        private PoolDatabaseHelper() : base("pool", new ScriptHelperData
        (
             columns: new string[] { "id", "game_id", "instance_name" },
             outputColumns: new string[] { "id", "game_id", "instance_name" },
             matchingColumns: new string[] { "id" },
             inequalityColumns: new string[0],
             updateColumns: new string[0],
             insertColumns: new string[] { "id", "game_id", "instance_name" }
        ))
        {

        }
    }
}
