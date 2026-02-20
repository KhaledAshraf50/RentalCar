using CarRental.Service.Interfaces;
using CarRental.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRental.Controllers
{
    [AllowAnonymous]

    public class AccountController : Controller
    {
        readonly IUserService _userService;
        readonly ILogger<AccountController> _logger;
        public AccountController(IUserService userService,ILogger<AccountController> logger)
        {
            _userService = userService; 
            _logger = logger;
        }
        // Get: /Account/Login (Partial View For popup)
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "HomePage");
            }
            return PartialView("_LoginPartial");
        }
        // Post: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = new Dictionary<string, string>();
                foreach(var key in ModelState.Keys)
                {
                    var error = ModelState[key].Errors.FirstOrDefault();
                    if (error != null)
                    {
                        errors[key] = error.ErrorMessage;
                    }
                }
                return Json(new { success = false, errors = errors });
            }
            try
            {
                var isValid = await _userService.ValidateUserAsync(model.Email, model.Password);
                if (!isValid)
                {
                    return Json(new { success = false, fieldErrors = new Dictionary<string,string>
                    {

                        { "general","Invalid Email or Password"}
                    }
                    });
                }
                var user = await _userService.GetUserByEmailAsync(model.Email);
                if(user == null)
                {
                    return Json(new { success = false, fieldErrors = new Dictionary<string, string> 
                    {
                        { "general", "User Not Found" }
                    } 
                    });
                }
                // Set session
                //HttpContext.Session.SetInt32("UserId", user.UserId);
                //HttpContext.Session.SetString("UserEmail", user.Email);
                //HttpContext.Session.SetString("UserName",user.FullName);
                //HttpContext.Session.SetString("UserRole", user.Role.Trim());
                //HttpContext.Session.SetString("ProfileImage", user.ProfileImage ?? "");
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                    new Claim(ClaimTypes.Name,user.FullName),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(ClaimTypes.Role,user.Role.Trim()),
                    new Claim("ProfileImage",user.ProfileImage??"")
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                };
                // Set cookie for remember me 
                //if (model.RememberMe)
                //{
                //    var cookieOptions = new CookieOptions
                //    {
                //        Expires = DateTime.Now.AddDays(30),
                //        HttpOnly = true,
                //        IsEssential = true
                //    };
                //    Response.Cookies.Append("UserEmail",user.Email, cookieOptions);
                //}
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(claimsIdentity),authProperties);
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Index","HomePage")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Login error for email: {Email}",model.Email);
                return Json(new
                {
                    success = false,
                    fieldErrors = new Dictionary<string, string>
                    {
                        { "general", "An error occurred during login" }
                    }
                });
            }
        }
        // GET: /Account/Register (Partial View for Popup)
        public IActionResult Register()
        {
            //if (HttpContext.Session.GetInt32("UserId") != null)
            //{
            //    return RedirectToAction("Index", "HomePage");
            //}
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "HomePage");
            }
            return PartialView("_RegisterPartial");
        }
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = new Dictionary<string, string>();
                foreach (var key in ModelState.Keys)
                {
                    var error = ModelState[key].Errors.FirstOrDefault();
                    if (error != null)
                    {
                        errors[key] = error.ErrorMessage;
                    }
                }
                return Json(new { success = false, fieldErrors = errors });
            }
            try
            {
                // Convert RegisterViewModel to UserViewModel
                var userViewModel = new UserViewModel
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone,
                    NewPassword = model.Password,
                    Role = "User" //Default role
                };
                var user = await _userService.RegisterUserAsync(userViewModel);
                // Auto login after registration
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role.Trim()),
                        new Claim("ProfileImage", user.ProfileImage ?? "")
                    };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );

                return Json(new
                {
                    Success = true,
                    redirectUrl = Url.Action("Index", "HomePage")
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    fieldErrors = new Dictionary<string, string>
            {
                { "Email", ex.Message }
            }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration error for email: {Email}", model.Email);
                return Json(new
                {
                    success = false,
                    fieldErrors = new Dictionary<string, string>
            {
                { "general", "An error occurred during Registration" }
            }
                });
            }
        }
        // Get: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            //// clear Session 
            //HttpContext.Session.Clear();
            //// clear remember me Cookie 
            //Response.Cookies.Delete("UserEmail");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "HomePage");
        }
        // Get: /Account/Profile
        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }
            var user = await _userService.GetByIdAsync(userId);
            if(user == null)
            {
                return RedirectToAction("Logout");
            }
            var model = new UserViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                Role = user.Role,
                ProfileImage = user.ProfileImage,
            };
            return View(model);
        }
        // Post: /Account/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile",model);
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }
            try
            {
                var success = await _userService.UpdateUserProfileAsync(userId, model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully";
                    HttpContext.Session.SetString("UserName", model.FullName);
                    return RedirectToAction("Profile");
                }
                ModelState.AddModelError("", "Failed to update profile");
                return View("Profile", model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Email", ex.Message);
                return View("Profile", model);
            }
        }
        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(UserViewModel model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout", "Account");
            }

            if (string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                ModelState.AddModelError("", "Current and new password are required");
                return View("Profile", model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                return View("Profile", model);
            }
            var success = await _userService.ChangePasswordAsync(userId , model.CurrentPassword, model.NewPassword);
            if (success)
            {
                TempData["SuccessMessage"] = "Password changed successfully";
                return RedirectToAction("Profile");
            }
            ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
            return View("Profile", model);
        }
        // AJAX: Check if email exists
        [HttpPost]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            var exists = await _userService.GetUserByEmailAsync(email) !=null;
            return Json(new {exists=exists});
        }
    }
}
