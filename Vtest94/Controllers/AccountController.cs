using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vtest94.Models;
using Vtest94.ViewModels;
using Vtest94.Utilities;
using Microsoft.EntityFrameworkCore;
using Vtest94.Data;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Vtest94.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage)
                                   .ToList();
                return BadRequest(new { Errors = errors });
                //return View(videoViewModel);
            }

            if (ModelState.IsValid)
            {
                // This is helper code for if user's UserName different from email address

                /*var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // User not found
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }

                // Check if email is confirmed (if required)
                if (_userManager.Options.SignIn.RequireConfirmedEmail && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "You need to confirm your email before logging in.");
                    return View(model);
                }*/

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Video");
                }
                else
                {
                    if (result.IsLockedOut)
                    {
                        ModelState.AddModelError(string.Empty, "This account has been locked out. Please try again later.");
                    }
                    else if (result.RequiresTwoFactor)
                    {
                        ModelState.AddModelError(string.Empty, "This account requires two-factor authentication.");
                    }
                    else if (result.IsNotAllowed)
                    {
                        ModelState.AddModelError(string.Empty, "You are not allowed to login. Please confirm your email first.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    }
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public IActionResult LoginWithGoogle(string returnUrl = "/Video/Index")
        {
            var redirectUrl = Url.Action(nameof(GoogleLoginCallback), "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }


        [HttpGet]
        public async Task<IActionResult> GoogleLoginCallback(string returnUrl = "/Video/Index", string remoteError = null)
        {
            if (remoteError != null) {
                return RedirectToAction("Login", new { error = $"Error from external provider: {remoteError}", errorCode = 1 });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // Consider logging this error
                //return RedirectToAction(nameof(Login));

                return RedirectToAction("Login", new { error = "Error loading external login information.", errorCode = 2 });
            }

            // Attempt to sign in the user with the external login info
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            // If user is not in our system, create a new user account
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                //user = new User { UserName = email, Email = email };
                //var createUserResult = await _userManager.CreateAsync(user);
                //if (!createUserResult.Succeeded)
                //{
                //    // Consider logging this error and showing a friendly message to the user
                //    return RedirectToAction(nameof(Login));
                //}
                return RedirectToAction("Login", new { error = "This account does not exist.", errorCode = 3 });
            }

            // Link the external login to the user account and sign in
            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded)
            {
                // Consider logging this error
                //return RedirectToAction(nameof(Login));
                return RedirectToAction("Login", new { error = "Failed to link external login. Please try again.", errorCode = 4 });
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return Redirect(returnUrl);
        }


        public IActionResult AccessDenied()
        {
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var aUsername = await UniqueUsername(model.FirstName, model.LastName);

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    ArbitraryUsername = aUsername
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Set default profile photo
                    var defaultPhotoPath = Path.Combine(_webHostEnvironment.WebRootPath, "auxiliary", "general-user-photo.jpg");
                    var userPhotoDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "UserPhotos", user.Id);
                    Directory.CreateDirectory(userPhotoDirectory);
                    var userPhotoPath = Path.Combine(userPhotoDirectory, "profile.jpg");

                    System.IO.File.Copy(defaultPhotoPath, userPhotoPath, true);

                    var userPhoto = new UserPhoto
                    {
                        FileName = "profile.jpg",
                        FilePath = $"/UserPhotos/{user.Id}/profile.jpg",
                        UserId = user.Id
                    };

                    _context.UserPhotos.Add(userPhoto);
                    await _context.SaveChangesAsync();

                    user.UserPhoto = userPhoto;
                    await _userManager.UpdateAsync(user);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                // Handle the error scenario
                return RedirectToAction(nameof(Login), new { ErrorMessage = "Error from external provider: " + remoteError });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // Handle the error scenario
                return RedirectToAction(nameof(Login), new { ErrorMessage = "Error loading external login information." });
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl ?? "/");
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);  // Extracting first name
                var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);    // Extracting last name
     
                var username = await UniqueUsername(firstName, lastName);

                var user = new User { UserName = email, Email = email, FirstName = firstName, LastName = lastName, ArbitraryUsername = username };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        // Set default profile photo
                        var defaultPhotoPath = Path.Combine(_webHostEnvironment.WebRootPath, "auxiliary", "general-user-photo.jpg");
                        var userPhotoDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "UserPhotos", user.Id);
                        Directory.CreateDirectory(userPhotoDirectory);
                        var userPhotoPath = Path.Combine(userPhotoDirectory, "profile.jpg");

                        System.IO.File.Copy(defaultPhotoPath, userPhotoPath, true);

                        var userPhoto = new UserPhoto
                        {
                            FileName = "profile.jpg",
                            FilePath = $"/UserPhotos/{user.Id}/profile.jpg",
                            UserId = user.Id
                        };

                        _context.UserPhotos.Add(userPhoto);
                        await _context.SaveChangesAsync();

                        user.UserPhoto = userPhoto;
                        await _userManager.UpdateAsync(user);

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        //return LocalRedirect(returnUrl ?? "/");
                        return RedirectToAction("Login", "Account");
                    }
                }

                return RedirectToAction(nameof(Login), new { ErrorMessage = "Failed to create the user account." });
            }
        }


        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Video");
        }

        private async Task<string> UniqueUsername(string firstName, string lastName)
        {
            string baseUsername = $"{firstName}{lastName}";
            string username = baseUsername;
            int counter = 1;

            while (await _userManager.FindByNameAsync(username) != null)
            {
                username = $"{baseUsername}{counter}";
                counter++;
            }

            return username;
        }
    }
}
