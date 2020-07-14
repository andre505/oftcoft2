using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OFTENCOFTAPI.Models;
using OFTENCOFTAPI.Models.User;
using OFTENCOFTAPI.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(LoginViewModel model);
        IEnumerable<ApplicationUser> GetAll();
    } 

    public class UserService : IUserService
    {
        private List<ApplicationUser> _users = new List<ApplicationUser>
        {
            new ApplicationUser { Id = 1.ToString(), FirstName = "Test", LastName = "User", UserName = "test", PasswordHash = "testpassword" }
        };

        private readonly AppSettings _appSettings;
        private readonly OFTENCOFTDBContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        UserManager<ApplicationUser> _userManager;
        public UserService(IOptions<AppSettings> appSettings, OFTENCOFTDBContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;

        }

        public async Task<AuthenticateResponse> Authenticate(LoginViewModel model)
        {
            string token = "";
            ApplicationUser user1 = new ApplicationUser();
            try
            {
                model.RememberMe = false;
                ApplicationUser user = await _userManager.FindByEmailAsync(model.email);

                if ((user != null))
                {
                    var result = await _signInManager.PasswordSignInAsync(model.email, model.password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        user1 = user;
                        token = generateJwtToken(user1);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            // authentication successful so generate jwt token
           

            return new AuthenticateResponse(user1, token);
        }

        //get users
        public IEnumerable<ApplicationUser> GetAll()
        {
            return _users;
        }

        //generate token
        private string generateJwtToken(ApplicationUser user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1).AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
