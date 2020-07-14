using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.Models.ViewModels
{
    public class PayRequest
    {
        public string email { get; set; }
        public string amount { get; set; }
    }
    public class TicketRequest
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string phonenumber { get; set; }
        public string itemname { get; set; }
        public string quantity { get; set; }
    }
}
