using SportStore.Models;
using SportStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;

namespace SportStore.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public UserManager<Customer> UserManager { get; }
        public SignInManager<Customer> SignInManager { get; }

        public AccountController(UserManager<Customer> userManager, SignInManager<Customer> signInManager, RoleManager<IdentityRole> RoleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            roleManager = RoleManager;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewBag.Roles = new SelectList(roleManager.Roles.Select(r => r.Name));
            return View();
        }

    
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        private bool ValidatePassword(string password, out string ErrorMessage)
        {
            var input = password;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new Exception("Password should not be empty");
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{6,20}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                ErrorMessage = "Пароль должен иметь хотя бы одну букву в нижнем регистре ";
                return false;
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                ErrorMessage = "Пароль должен иметь хотя бы одну букву в верхнем регистре";
                return false;
            }
            else if (!hasMiniMaxChars.IsMatch(input))
            {
                ErrorMessage = "Пароль должен быть длиной больше 6 и меньше 20 букв";
                return false;
            }
            else if (!hasNumber.IsMatch(input))
            {
                ErrorMessage = "Пароль должен иметь хотя бы одну цифру";
                return false;
            }
            /*
            else if (!hasSymbols.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one special case characters";
                return false;
            }
            */
            else
            {
                return true;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model, string ReturnUrl)
        {
            string ErrorMessage;
            bool isValid = ValidatePassword(model.Password, out ErrorMessage);
            if (!isValid) 
            {
                ModelState.AddModelError("Password", ErrorMessage);
                ViewBag.Roles = new SelectList(roleManager.Roles.Select(r => r.Name));
                return View(model);
            }
            
            
            if (ModelState.IsValid)
            {
                var user = new Customer { UserName = model.Name, Email = model.Email, City = model.City };
                var result = await UserManager.CreateAsync(user, model.Password);
                IdentityResult autorize_result;


                if (model.Role != null && ModelState.IsValid)
                {
                   
                    autorize_result = await UserManager.AddToRoleAsync(user, model.Role);

                    if (!autorize_result.Succeeded) 
                    {
                        foreach (var error in autorize_result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        ViewBag.BadMessage = "Произошла ошибка";
                        ViewBag.Roles = new SelectList(roleManager.Roles.Select(r => r.Name));
                        return View(model);
                        
                    }
                }
                
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false);

                    

                    if (ReturnUrl != null)
                    {
                        return RedirectToAction(ReturnUrl);
                    }
                    return RedirectToAction("index", "home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
               
            }
            ViewBag.Roles = new SelectList(roleManager.Roles.Select(r => r.Name));
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    TempData["BadMessage"] = "Такого пользователя нет";
                    return View(model);
                }
                var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return RedirectToAction(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }
                ModelState.AddModelError(String.Empty, "Invalid Log in attempt ");
                
            }
            return View(model);
        }

        [HttpPost]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string Email)
        {
            var user = await UserManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Почта {Email} уже используеться");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) 
        {
            string ErrorMessage;
            bool flag = ValidatePassword(model.NewPassword, out ErrorMessage);
            if (!flag)
            {
                ModelState.AddModelError("NewPassword", ErrorMessage);
                return View(model);
            }

            if (ModelState.IsValid) {
                var user = await UserManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login");

                var result = await UserManager.ChangePasswordAsync(user, model.CurrentPassword,
                    model.NewPassword);

                if (!result.Succeeded) 
                {
                    foreach (var error in result.Errors) 
                    {
                        ModelState.AddModelError(string.Empty, error.Description);                    
                    }
                    return View();
                }

                await SignInManager.RefreshSignInAsync(user);
                return RedirectToAction("Profile");
            }
            return View(model);

        }
        public async Task<IActionResult> Profile()
        {
            Customer customer = await GetCurrentUserAsync();
            if (customer == null) return RedirectToAction("Login");

            ProfileViewModel model = new ProfileViewModel()
            {
                Id = customer.Id,
                Name = customer.UserName,
                Location = customer.City,
                Email = customer.Email
            };
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> ChangePersonalData(ProfileViewModel model) 
        {
            if (ModelState.IsValid)
            {
                Customer customer = await GetCurrentUserAsync();
                customer.UserName = model.Name;
                customer.Email = model.Email;
                customer.City = model.Location;

                IdentityResult result = await UserManager.UpdateAsync(customer);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("Profile", model);
                }
                return RedirectToAction("Profile");
            }
            return View("Profile", model);

        }

        private Task<Customer> GetCurrentUserAsync() => UserManager.GetUserAsync(HttpContext.User);

    }
}
