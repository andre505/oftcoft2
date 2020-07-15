using System.Threading.Tasks;
using OFTENCOFTAPI.Services;
using Microsoft.AspNetCore.Identity;
using OFTENCOFTAPI.ApplicationCore.Models.User;
using Microsoft.Extensions.Options;
using OFTENCOFTAPI.Data.Models;
using OFTENCOFTAPI.ApplicationCore.Interfaces;
using OFTENCOFTAPI.ApplicationCore.DTOs;
using OFTENCOFTAPI.ApplicationCore.Utils;

namespace OFTENCOFTAPI.ApplicationCore.Services
{
    public class IdentityService : IIdentityService
    {
        private IUserService _userService;
        private readonly OFTENCOFTDBContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppSettings _appSettings;

        public IdentityService(IUserService userService, OFTENCOFTDBContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        public async Task<LoginResultDTO> LoginAsync(string email, string password, bool rememberMe)
        {
            string token = "";

            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    string secret = _appSettings.Secret;
                    token = IdentityUtil.generateJwtToken(user, secret);

                    UserSignInResultDTO customerResult = new UserSignInResultDTO
                    {
                        firstname = user.FirstName,
                        lastname = user.LastName,
                        email = user.Email,
                        phone = user.PhoneNumber,
                        token = token
                    };

                    LoginResultDTO loginResultDTO = new LoginResultDTO
                    {
                        Status = "success",
                        ResponseCode = "00",
                        ResponseMessage = "Sign in Successfully",
                        userSignInResult = customerResult,
                        ErrorList = null
                    };

                    return loginResultDTO;
                } 
                else
                {
                    LoginResultDTO loginResultDTO = new LoginResultDTO
                    {
                        Status = "fail",
                        ResponseCode = "01",
                        ResponseMessage = "Invalid Credentials",
                        userSignInResult = null,
                        ErrorList = null
                    };

                    return loginResultDTO;
                }
            }
            else
            {
                LoginResultDTO loginResultDTO = new LoginResultDTO
                {
                    Status = "fail",
                    ResponseCode = "01",
                    ResponseMessage = "User does not exist",
                    userSignInResult = null,
                    ErrorList = null
                };

                return loginResultDTO;
            }
           
        }

        public Task<AuthenticationResultDTO> RegisterAsync()
        {
            return null;
        }

    }
}
