using System;
using System.Collections.Generic;

#nullable disable

namespace DataReceiver.Shared.Database
{
    public partial class Combination
    {
        public long id { get; set; }
        public int pool_id { get; set; }
        public short combination_id { get; set; }
        public decimal sales { get; set; }
        public decimal liability { get; set; }
        public decimal? odds { get; set; }
        public long investment_number { get; set; }
        public long odds_number { get; set; }
        public DateTime create_time { get; set; }
    }
}
