using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.Models
{
    public class ItemDraw
    {
        public int catid { get; set; }
        public string itemname { get; set; }
        public int qty { get; set; }
        public string drawdate { get; set; }
        public decimal? amount { get; set; }
        public string itemdescription { get; set; }
        public DrawType drawtype { get; set; }
        public DrawStatus drawstatus { get; set; }
    }
}
