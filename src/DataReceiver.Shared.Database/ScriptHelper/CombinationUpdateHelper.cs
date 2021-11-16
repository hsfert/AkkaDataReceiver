using SQLScriptHelper;

namespace DataReceiver.Shared.Database.ScriptHelper
{
    public class CombinationUpdateHelper : MergeScriptHelper<Combination>
    {
        public static CombinationUpdateHelper Instance = new CombinationUpdateHelper();

        private CombinationUpdateHelper() : base("combination", new ScriptHelperData
        (
             columns: new string[] { "id", "sales", "liability", "investment_number", "odds", "odds_number" },
             outputColumns: new string[0],
             matchingColumns: new string[] { "id" },
             inequalityColumns: new string[0],
             updateColumns: new string[] {  "sales", "liability", "investment_number", "odds", "odds_number" },
             insertColumns: new string[0]
        ))
        {

        }
    }
}
