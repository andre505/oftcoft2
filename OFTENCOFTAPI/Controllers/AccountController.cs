using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OFTENCOFTAPI.Models;
using OFTENCOFTAPI.Models.User;
using OFTENCOFTAPI.ViewModels.Account;

namespace OFTENCOFTAPI.Controllers
{
    [Authorize (Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IConfiguration _configuration;
        private readonly OFTENCOFTDBContext _context;
        private readonly ILogger _logger;
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, OFTENCOFTDBContext context, ILogger<AccountController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        [Authorize(Roles = "Admin, Operations")]
        public IActionResult Index()
        {
            return View();
        }

        ///
        //Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            _logger.LogInformation("Welcome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            model.RememberMe = false;
            string defaultpass = _configuration.GetSection("DefaultPassword").Value;
            try
            {
                if (ModelState.IsValid)
                { 
                    ApplicationUser user = await _userManager.FindByEmailAsync(model.email);

                    if ((user != null) )
                    {
                        
                        if (user.EmailConfirmed == true)
                        {
                            var result = await _signInManager.PasswordSignInAsync(model.email, model.password, model.RememberMe, lockoutOnFailure: false);
                            if (result.Succeeded)
                            {
                                ViewBag.loggedin = "success";
                                _logger.LogInformation("User {0} logged in at {1}", model.email, DateTime.UtcNow.AddDays(1).ToString());
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            if (model.password != defaultpass)
                            {
                                ViewBag.error = "true";
                                ViewBag.message = "Invalid Login Attempt";
                                ModelState.AddModelError(string.Empty, "Invalid login attempt. Please enter valid login credentials");
                                return View(model);
                            }
                            //CompleteRegistrationViewModel
                            return RedirectToAction("CompleteRegistration", "Account", new {email = model.email } );
                        }             

                    }

                    else
                    {
                        ViewBag.loggedin = "fail";
                        ViewBag.error = "true";
                        ViewBag.message = "Invalid Login Attempt";
                        ModelState.AddModelError(string.Empty, "Invalid login attempt. Please enter valid login credentials");
                        return View(model);
                    }
                }
            }
            catch (Exception /* ex */)
            {
                ViewBag.loading = "not loading";
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to Complete Request");
                ViewBag.error = "true";
                ViewBag.message = "A Network Error Occurred";
            }
            return View(model);
        }


        //Complete Registration
        [AllowAnonymous]
        public IActionResult CompleteRegistration(string email)
        {
            CompleteRegistrationViewModel model = new CompleteRegistrationViewModel();
            model.Email = email;
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteRegistration(CompleteRegistrationViewModel model)
        {
            model.RememberMe = false;
            try
            {                
                    if (ModelState.IsValid)
                    {
                        ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);
                        var defaultpassword = _configuration.GetSection("DefaultPassword").Value;
                        //if (_userManager.FindByEmailAsync(model.Email).Result == null)

                        if (user != null)
                        {
                                if (model.Password == defaultpassword)
                                {
                                    ViewBag.error = "true";
                                    ViewBag.message = "Old password cannot be the same as new password";
                                    return View(model);
                                }
                                                               
                                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                                var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, model.Password);

                                if (resetResult.Succeeded)
                                {
                                    user.EmailConfirmed = true;
                                    await  _userManager.UpdateAsync(user);
                                    await _context.SaveChangesAsync();
                                    var signInResult = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                                    if (signInResult.Succeeded)
                                    {
                                        ViewBag.error = "false";
                                        ViewBag.loggedin = "success";
                                        _logger.LogInformation("User {0} logged in at {1}", model.Email, DateTime.UtcNow.AddDays(1).ToString());
                                        return RedirectToAction("Index", "Home");
                                    }
                                }                        

                                else
                                {
                                    ViewBag.error = "true";
                                    ViewBag.message = "An error occurred, password could not be reset";
                                    ModelState.AddModelError(string.Empty, "An error occurred, password could not be reset");
                                    return View(model);
                                }
                        }
                        else
                        {                  
                          
                                ViewBag.error = "true";
                                ViewBag.message = "Invalid Credentials";
                                return View(model);                                                         
                        }
                    }             
            }
            catch (Exception ex)
            {
                ViewBag.loading = "not loading";
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to Complete Request");
                ViewBag.error = "true";
                ViewBag.message = "A Network Error Occurred";
            }
            return View(model);
        }

        //Create Operator
        public IActionResult CreateOperator()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateOperator(CreateUserViewModel model)
        {

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
                        user.EmailConfirmed = false;
                        string password = _configuration.GetSection("DefaultPassword").Value;
                        IdentityResult result = _userManager.CreateAsync(user, password).Result;

                        if (result.Succeeded)
                        {
                            _userManager.AddToRoleAsync(user, "Operations").Wait();
                        }
                        else
                        {
                            ViewBag.error = "true";
                            ViewBag.message = "An error occurred";
                            return View(model);
                        }
                    }
                    else
                    {
                        ViewBag.error = "true";
                        ViewBag.message = "Email already exists";
                        ModelState.AddModelError(string.Empty, "An error occurred");
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.loading = "not loading";
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to Complete Request");
                ViewBag.error = "true";
                ViewBag.message = "A Network Error Occurred";
            }
            return RedirectToAction("ViewOperators");
        }

        public async Task<IActionResult> ViewOperators()
        {
            // return View(await _context.Users.ToListAsync());
            var FloorMembers = await _userManager.GetUsersInRoleAsync("FloorMember");
            var Operators = await _userManager.GetUsersInRoleAsync("Operations");
            var total = FloorMembers.Concat(Operators);
            return View(FloorMembers);
        }

        //logout
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User " + user.Email + "Signed Out");
            return RedirectToAction("Login", "Account");
        }


    }
}