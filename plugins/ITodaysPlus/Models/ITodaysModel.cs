using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITodaysPlus.Models
{
    public class ITodaysModel
    {
        public long Num { get; set; }

        public string Target { get; set; }


        public string Type { get; set; }


        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public long SpendSecond { get; set; }

        public string Remark { get; set; }


    }
}
