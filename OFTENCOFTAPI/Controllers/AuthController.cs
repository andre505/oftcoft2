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
        public AuthController(IUserService userService, OFTENCOFTDBContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
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
                if (ModelState.IsValid)
                {
                    if (_userManager.FindByEmailAsync(model.email).Result == null)
                    {
                        ApplicationUser user = new ApplicationUser();
                        user.UserName = model.email;
                        user.Email = model.email;
                        user.FirstName = model.firstname;
                        user.LastName = model.lastname;
                        user.EmailConfirmed = true;
                        user.PhoneNumber = model.phonenumber;
                        IdentityResult result = _userManager.CreateAsync(user, model.Password).Result;

                        if (result.Succeeded)
                        {

                            UserSignInResult customerResult = new UserSignInResult
                            {
                                firstname = model.firstname,
                                lastname = model.lastname,
                                email = model.email,
                                phone = model.phonenumber
                            };
                            _userManager.AddToRoleAsync(user, "Customer").Wait();

                            var signinresult = await _signInManager.PasswordSignInAsync(model.email, model.Password, model.rememberMe, lockoutOnFailure: false);
                            if (signinresult.Succeeded)
                            {
                                string token = generateJwtToken(user);
                                //await _signInManager.SignInAsync(user, isPersistent: false);
                                customerResult.token = token;

                                //success
                                var RegisterSuccessResponse = new
                                {
                                    status = "success",
                                    responsecode = "00",
                                    responsemessage = "User Registration Successful",
                                    userdetails = customerResult,
                                };

                                return new JsonResult(RegisterSuccessResponse);
                            }

                            else
                            {
                                var RegisterSuccessResponse = new
                                {
                                    status = "success",
                                    responsecode = "00",
                                    responsemessage = "User Registration Successful. Login to retrieve token",
                                    userdetails = customerResult,
                                };
                                return new JsonResult(RegisterSuccessResponse);
                            }
                        }
                        else
                        {
                            //fail
                            var RegisterFailureResponse = new
                            {
                                status = "fail",
                                responsecode = "01",
                                responsemessage = "User Registration Failed. Please try again later",
                                userdetails = "",
                            };

                            return new JsonResult(RegisterFailureResponse);

                        }
                    }
                    else
                    {
                        //user exists
                        var RegisterFailureResponse = new
                        {
                            status = "fail",
                            responsecode = "01",
                            responsemessage = "User registration failed. User already exists",
                            userdetails = "",
                        };
                        return new JsonResult(RegisterFailureResponse);

                    }

                }
                //invalid model state
                else
                {
                    var query = from state in ModelState.Values
                                from error in state.Errors
                                select error.ErrorMessage;

                    var errorList = query.ToList();

                    var ModelErrors = new
                    {
                        status = "fail",
                        responsecode = "01",
                        responsemessage = "User Registration Failed",
                        errors = errorList
                    };

                    return new JsonResult(ModelErrors);

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
                if (ModelState.IsValid)
                {
                    string token = "";
                    ApplicationUser user = await _userManager.FindByEmailAsync(model.email);
                    if ((user != null))
                    {

                        var result = await _signInManager.PasswordSignInAsync(model.email, model.password, model.RememberMe, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            token = generateJwtToken(user);

                            UserSignInResult customerResult = new UserSignInResult
                            {
                                firstname = user.FirstName,
                                lastname = user.LastName,
                                email = user.Email,
                                phone = user.PhoneNumber,
                                token = token
                            };

                            var signinresponse = new
                            {
                                status = "success",
                                responsecode = "00",
                                responsemessage = "Sign in Successful",
                                user = customerResult
                            };
                            return new JsonResult(signinresponse);
                        } 
                        else
                        {
                            var signinfailresponse = new
                            {
                                status = "fail",
                                responsecode = "01",
                                responsemessage = "Invalid credentials",
                                user = ""
                            };
                            return new JsonResult(signinfailresponse);

                        }
                    }
                    else
                    {
                        var signinfailresponse = new
                        {
                            status = "fail",
                            responsecode = "01",
                            user = "User does not exist"
                        };
                        return new JsonResult(signinfailresponse);
                    }
                }
                //model state invalid 
                else
                {
                    var query = from state in ModelState.Values
                                from error in state.Errors
                                select error.ErrorMessage;

                    var errorList = query.ToList();

                    var ModelErrors = new
                    {
                        status = "fail",
                        responsecode = "01",
                        responsemessage = "Sign in failed",
                        errors = errorList
                    };

                    return new JsonResult(ModelErrors);

                }
            }
            catch(Exception ex)
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