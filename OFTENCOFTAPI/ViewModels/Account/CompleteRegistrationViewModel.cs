using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.ViewModels.Account
{
    public class CompleteRegistrationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Default Password")]
        //public string OldPassword { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 7, ErrorMessage = "field must be at least 7 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [StringLength(1000, MinimumLength = 7, ErrorMessage = "field must be at least 7 characters")]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool RememberMe { get; set; }
    }
}
