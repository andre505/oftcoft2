using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.ApplicationCore.DTOs
{
    public class PasswordResetDTO
    {
        public string NewPassword { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
