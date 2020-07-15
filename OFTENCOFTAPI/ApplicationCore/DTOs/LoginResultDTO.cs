using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.ApplicationCore.DTOs
{
    public class LoginResultDTO
    {
        public string Status { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public UserSignInResultDTO UserSignInResult { get; set; }
        public IList<string> ErrorList { get; set; }
    }
}
