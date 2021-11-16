using SQLScriptHelper;

namespace DataReceiver.Shared.Database.ScriptHelper
{
    public class CombinationCreationHelper : MergeScriptHelper<Combination>
    {
        public static CombinationCreationHelper Instance = new CombinationCreationHelper();
        private CombinationCreationHelper() : base("combination", new ScriptHelperData
        (
             columns: new string[] {"pool_id", "combination_id", "sales", "liability", "odds", "investment_number", "odds_number" },
             outputColumns: new string[] { "id", "pool_id", "combination_id", "sales", "liability", "odds", "investment_number","odds_number" },
             matchingColumns: new string[] {  "pool_id", "combination_id" },
             inequalityColumns: new string[0],
             updateColumns: new string[0],
             insertColumns: new string[] { "pool_id", "combination_id", "sales", "liability", "odds", "investment_number", "odds_number" }
        ))
        {

        }
    }
}
