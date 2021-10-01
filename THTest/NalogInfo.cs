using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace THTest
{
    public class NalogInfo
    {
        [JsonIgnore]
        public DateTime LastSearchingTime 
        {
            get
            {
                return DateTime.Parse(dtQueryEnd);
            }
        }
        public int pageCount { get; set; }
        public string dtQueryEnd { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public NalogInfoData[] data { get; set; }
        public string dtQueryBegin { get; set; }
        public bool queryCount { get; set; }
        public int rowCount { get; set; }
        public int rowLimit { get; set; }
        public int queryTime { get; set; }
    }
}
