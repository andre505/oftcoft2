using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.ApplicationCore.DTOs
{
    public class UserSignInResultDTO
    {
        public string firstname { get; set; }


        public string lastname { get; set; }


        public string email { get; set; }


        public string token { get; set; }


        public string phone { get; set; }
    }
}
