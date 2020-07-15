using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.ViewModels.Account
{
    public class CreateUserViewModel
    {
        [Required]

        public string firstname { get; set; }

        [Required]
        public string lastname { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }
    }

    public class RegisterCustomerViewModel
    {
        [Required]
        public string firstname { get; set; }
        
        [Required]
        public string lastname { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string phonenumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Password must be at least 7 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool rememberMe { get; set; }
    }


    public class UserSignInResult
    {
        public string firstname { get; set; }


        public string lastname { get; set; }


        public string email { get; set; }


        public string token { get; set; }


        public string phone { get; set; }

    }
}
