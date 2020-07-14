using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OFTENCOFTAPI.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.Data
{
    public class UsersandRolesInitializer
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {

            if (userManager.FindByEmailAsync("admin@nationaluptake.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "admin@nationaluptake.com";
                user.Email = "admin@nationaluptake.com";
                user.FirstName = "Admin";
                user.LastName = "NationalUptake";
                user.EmailConfirmed = false;
                IdentityResult result = userManager.CreateAsync(user, "abxp95p").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();                    
                }
            }

            //////////
            if (userManager.FindByEmailAsync("operations@nationaluptake.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "operations@nationaluptake.com";
                user.Email = "operations@nationaluptake.com";
                user.FirstName = "Operations";
                user.LastName = "NationalUptake";
                user.EmailConfirmed = false;
                IdentityResult result = userManager.CreateAsync(user, "abxp95p").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Operations").Wait();
                }
            }

        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {

            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Operations").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Operations";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync("Customer").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Customer";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
        }
    }
}
