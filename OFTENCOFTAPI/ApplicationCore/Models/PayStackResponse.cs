using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.ApplicationCore.Models
{
    public class PayStackResponse
    {

        public string Status { get; set; }

        public string Message { get; set; }

        public string AuthorizationUrl { get; set; }

        public string AccessCode { get; set; }

        public string Reference { get; set; }
    }
}
