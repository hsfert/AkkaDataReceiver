using System;
using System.Collections.Generic;

#nullable disable

namespace DataReceiver.Shared.Database
{
    public partial class Pool
    {
        public long id { get; set; }
        public int game_id { get; set; }
        public string instance_name { get; set; }
        public DateTime create_time { get; set; }
    }
}
