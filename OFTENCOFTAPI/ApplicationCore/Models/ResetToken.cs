using OFTENCOFTAPI.ApplicationCore.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.ApplicationCore.Models
{
    public class ResetToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiry { get; set; }
        public string UserId { get; set; }
    }
}
