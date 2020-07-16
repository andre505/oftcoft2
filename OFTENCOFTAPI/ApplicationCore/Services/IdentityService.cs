using System.Threading.Tasks;
using OFTENCOFTAPI.Services;
using Microsoft.AspNetCore.Identity;
using OFTENCOFTAPI.ApplicationCore.Models.User;
using Microsoft.Extensions.Options;
using OFTENCOFTAPI.Data.Models;
using OFTENCOFTAPI.ApplicationCore.Interfaces;
using OFTENCOFTAPI.ApplicationCore.DTOs;
using OFTENCOFTAPI.ApplicationCore.Utils;
using System;
using System.Linq;
using OFTENCOFTAPI.ApplicationCore.Models;

namespace OFTENCOFTAPI.ApplicationCore.Services
{
    public class IdentityService : IIdentityService
    {
        private IUserService _userService;
        private readonly OFTENCOFTDBContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IResetTokenRepository _resetTokenRepository;
        private readonly IEmailService _emailService;
        private readonly AppSettings _appSettings;

        public IdentityService(
            IUserService userService, 
                OFTENCOFTDBContext context,
                SignInManager<ApplicationUser> signInManager,
                UserManager<ApplicationUser> userManager,
                IOptions<AppSettings> appSettings,
                IResetTokenRepository resetTokenRepository,
                IEmailService emailService
            )
        {
            _userService = userService;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _resetTokenRepository = resetTokenRepository;
            _emailService = emailService;
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
                        UserSignInResult = customerResult,
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
                        UserSignInResult = null,
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
                    UserSignInResult = null,
                    ErrorList = null
                };

                return loginResultDTO;
            }
           
        }

        public async Task<RegistrationResultDTO> RegisterAsync(
            string email, string password, bool rememberMe, string lastname, string firstname, string phonenumber)
        {
            var isAccountExisting = await this.VerifyAccountExist(email);
            if (isAccountExisting)
            {

                RegistrationResultDTO registrationResultDTO = new RegistrationResultDTO
                {
                    Status = "fail",
                    ResponseCode = "01",
                    ResponseMessage = "User registration failed. User already exists",
                    UserSignInResult = null,
                    ErrorList = null
                };

                return registrationResultDTO;
            }

            ApplicationUser user = new ApplicationUser();
            user.UserName = email;
            user.Email = email;
            user.FirstName = firstname;
            user.LastName = lastname;
            user.EmailConfirmed = true;
            user.PhoneNumber = phonenumber;
            IdentityResult result = _userManager.CreateAsync(user, password).Result;

            if(result.Succeeded)
            {
                UserSignInResultDTO userSignInResult = new UserSignInResultDTO
                {
                    firstname = firstname,
                    lastname = lastname,
                    email = email,
                    phone = phonenumber
                };

                _userManager.AddToRoleAsync(user, "Customer").Wait();

                var signInResult = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
                if (signInResult.Succeeded)
                {
                    string secret = _appSettings.Secret;
                    string token = IdentityUtil.generateJwtToken(user, secret);

                    userSignInResult.token = token;

                    RegistrationResultDTO registrationResultDTO = new RegistrationResultDTO
                    {
                        Status = "success",
                        ResponseCode = "00",
                        ResponseMessage = "User registered successfully",
                        UserSignInResult = userSignInResult,
                        ErrorList = null
                    };

                    return registrationResultDTO;
                }
                else
                {

                    RegistrationResultDTO registrationResultDTO = new RegistrationResultDTO
                    {
                        Status = "success",
                        ResponseCode = "00",
                        ResponseMessage = "User Registration Successful. Login to retrieve token",
                        UserSignInResult = userSignInResult,
                        ErrorList = null
                    };

                    return registrationResultDTO;
                }
            }
            else
            {

                RegistrationResultDTO registrationResultDTO = new RegistrationResultDTO
                {
                    Status = "fail",
                    ResponseCode = "01",
                    ResponseMessage = "User Registration Failed. Please try again later",
                    UserSignInResult = null,
                    ErrorList = null
                };

                return registrationResultDTO;
            }
        }

        public async Task<bool> VerifyAccountExist(string email)
        {
            var userObject = await _userManager.FindByEmailAsync(email);

            return userObject != null;
        }

        public async Task<ResetRequestResultDTO> SendResetToken(string email)
        {
            // => Check if account exist
            ApplicationUser user = await _userManager.FindByEmailAsync(email);

            //     => Send error response
            if (user == null)
            {
                return new ResetRequestResultDTO
                {
                    Status = "failed",
                    Message = "Account doesn't exist"
                };
            }

            //, On existing
            //=> Generate reset token, 4 digit
            Random rnd = new Random();
            var token = GlobalUtil.GenerateDigit(rnd, 4);
            var expiry = DateTime.Now.AddMinutes(15);

            // => Add 15minutes to current time,
            // and set as PasswordTokenExpiryTime
            var resetToken = new ResetToken
            {
                Token = token,
                TokenExpiry = expiry,
                UserId = user.Id
            };

            try
            {
                //refactor to reset token respository
                await _resetTokenRepository.SaveToken(resetToken);

                //=> Send reset email
                var subject = "Password Reset Request";

                string message = "";
                string htmlMessage = @"<!DOCTYPE html>
                            <html>
                            <head>
                            <style>
                            </style>
                            </head>
                            <body>
                            <img style='display:block;' align='right' src='https://www.dropbox.com/s/0p1flnq0voo7hn9/oftcoftlogosmall.jpg?raw=1' alt = 'felt lucky'></a>" +
                                "<h3 style = 'font-family: Arial, sans-serif; font-size: 250%; color:#9370DB;'> Account Recovery </h3>" +
                                "<h5 style = 'font-family: Arial, sans-serif; font-size: 250%; color:#9370DB;'> Dear User </h5>" +
                                 "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> You recently requested to reset your password. Use the below 4 digit OTP. This token is only valid for the next 15 minutes. </p>" +
                                 "<p style = 'font-family: Gill Sans, sans-serif; font-size: 160%; color:#666666;'> Token : " + resetToken.Token + "</p>" +
                                 "<p></p>" +
                                 "<a href='https://www.nationalgiveaway.com'><img style='display:block; width:100%;height:100%;' src='https://www.dropbox.com/s/medm6f3npfr4gh5/freegift.jpg?raw=1' alt = 'feeling lucky'></a>" +
                                 "</body>" +
                                 "</html>";

                //=> Send response
               // await _emailService.ExecuteAsync(email, subject, message, htmlMessage);

                return new ResetRequestResultDTO
                {
                    Status = "success",
                    Message = "Password reset token sent"
                };

            }
            catch (Exception ex)
            {
                return new ResetRequestResultDTO
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }


        }

        public async Task<ResetRequestResultDTO> ResetPassword(string token, string newPassword, string email)
        {
            // verify token, if it matches
            var resetTokenInDb = _resetTokenRepository.FindToken(token);

            if (resetTokenInDb == null)
            {
                return new ResetRequestResultDTO
                {
                    Status = "failed",
                    Message = "Invalid Token"
                };
            }

            var isExpired = _resetTokenRepository.VerifyIfTokenExpired(resetTokenInDb);
            // verify it has not expired, if it has not
            if (isExpired)
            {
                return new ResetRequestResultDTO
                {
                    Status = "failed",
                    Message = "Token Expired"
                };
            }

            // update password
            var user = await _userManager.FindByEmailAsync(email);
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new ResetRequestResultDTO
                {
                    Status = "success",
                    Message = "Password Updated"
                };
            } else
            {

                return new ResetRequestResultDTO
                {
                    Status = "failed",
                    Message = "Something went wrong"
                };
            }

        }
    }
}
