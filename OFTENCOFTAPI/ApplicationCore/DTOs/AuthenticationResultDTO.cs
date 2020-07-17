using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.ApplicationCore.DTOs
{
    public class AuthenticationResultDTO
    {
        public string Status { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
