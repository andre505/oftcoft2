using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        public string email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        public bool RememberMe { get; set; }
    }

}
