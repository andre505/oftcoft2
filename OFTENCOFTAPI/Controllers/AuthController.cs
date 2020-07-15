using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using OFTENCOFTAPI.Services;
using OFTENCOFTAPI.ViewModels.Account;
using OFTENCOFTAPI.ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using OFTENCOFTAPI.ApplicationCore.Models.User;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OFTENCOFTAPI.Data.Models;
using OFTENCOFTAPI.ApplicationCore.Interfaces;
using OFTENCOFTAPI.ApplicationCore.DTOs;

namespace OFTENCOFTAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private readonly OFTENCOFTDBContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppSettings _appSettings;
        private readonly IIdentityService _identityService;

        public AuthController(IUserService userService, OFTENCOFTDBContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings, IIdentityService identityService)
        {
            _userService = userService;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _identityService = identityService;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]LoginViewModel model)
        {

            //var response = _userService.Authenticate(model);

            //if (response == null)
            //    return BadRequest(new { message = "Username or password is incorrect" });

            //return Ok(response);
            //====================================================
            string token = "";
            ApplicationUser user = await _userManager.FindByEmailAsync(model.email);
            if ((user != null))
            {

                if (user.EmailConfirmed == true)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.email, model.password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        token = generateJwtToken(user);

                        var data1 = new
                        {
                            status = "success",
                            user = model.email,
                            token = token
                        };

                        return new JsonResult(data1);
                    }
                }


            }
            var data = new
            {
                status = "fail, user does not exist",
                user = "",
                token = ""
            };

            return new JsonResult(data);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var users = _userService.GetAll();
                return Ok(users);
            }

            catch (Exception e)
            {
                return Forbid();
            }
        }

        //REGISTER
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterCustomerViewModel model)
        {
            model.rememberMe = false;

            try
            {
                if (!ModelState.IsValid)
                {

                    var query = from state in ModelState.Values
                                from error in state.Errors
                                select error.ErrorMessage;

                    var errorList = query.ToList();

                    return BadRequest(new RegistrationResultDTO {
                        Status = "fail",
                        ResponseCode = "01",
                        ResponseMessage = "User Registration Failed",
                        ErrorList = errorList
                    });
                }                
                else
                {
                    var registrationResult = await _identityService.RegisterAsync(model.email, model.Password, model.rememberMe, model.lastname, model.firstname, model.phonenumber);

                    if(registrationResult.Status == "success")
                    {
                        return Ok(registrationResult);
                    }
                    else
                    {
                        return BadRequest(registrationResult);
                    }
                }
            }
            catch (Exception ex)
            {
                var OtherErrors = new
                {
                    status = "fail",
                    responsecode = "02",
                    responsemessage = ex.Message.ToString(),
                    userdetails = model,
                };

                return new JsonResult(OtherErrors);

            }

        }
        //LOGIN

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            model.RememberMe = false;

            try
            {
                if (!ModelState.IsValid)
                {

                    var query = from state in ModelState.Values
                                from error in state.Errors
                                select error.ErrorMessage;

                    var errorList = query.ToList();

                    return BadRequest(new LoginResultDTO
                    {
                        Status = "fail",
                        ResponseCode = "01",
                        ResponseMessage = "Sign in failed",
                        userSignInResult = null,
                        ErrorList = errorList
                    });

                }

                var loginResult = await _identityService.LoginAsync(model.email, model.password, model.RememberMe);

                if(loginResult.Status == "success")
                {
                    return Ok(loginResult);
                } 
                    else
                {
                    return BadRequest(loginResult);
                }
            }
            catch (Exception ex)
            {

                var OtherErrors = new LoginResultDTO
                {
                    Status = "fail",
                    ResponseCode = "02",
                    ResponseMessage = ex.Message.ToString(),
                    userSignInResult = null,
                    ErrorList = null
                };

                return StatusCode(500, OtherErrors);
            }
                   

        }

        //GENERATE JWT TOKEN
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