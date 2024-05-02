using TaskManager.Contexts;
using TaskManager.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace TaskManager.Controllers
{
    public class UserController : Controller
    {

        private readonly UserContext _context;

        public UserController(UserContext context)
        {
            _context = context;
        }


        // GET: UserController
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user, IFormFile avatarFile)
        {
            if (ModelState.IsValid)
            {
                User? _user = _context.users.FirstOrDefault(x => x.Login == user.Login);
                if (_user == null)
                {
                    if (avatarFile != null && avatarFile.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await avatarFile.CopyToAsync(memoryStream);
                            user.Avatar = memoryStream.ToArray();
                        }
                    }

                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login");
                }
                TempData["ErrorMessage"] = "Логин уже занят.";
            }
            return View();
        }


        //[HttpPost]
        //public async Task<IActionResult> Register([Bind("Id, Login, Password, FirstName, LastName, Email")] User user, IFormFile avatar)
        //{
        //    if(ModelState.IsValid)
        //    {
        //        User? _user = _context.users.FirstOrDefault(x => x.Login == user.Login);
        //        if (_user == null) 
        //        {
        //            if(avatar != null && avatar.Length > 0)
        //            {
        //                using (MemoryStream ms = new MemoryStream())
        //                {
        //                    await avatar.CopyToAsync(ms);
        //                    user.Avatar = ms.ToArray();
        //                }
        //            }
        //            _context.Add(user);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction("Login");
        //        }
        //        TempData["ErrorMessage"] = "Логин уже занят.";

        //    }

        //    // Выводим ошибки валидации
        //    foreach (var modelStateEntry in ModelState.Values)
        //    {
        //        foreach (var error in modelStateEntry.Errors)
        //        {
        //            var errorMessage = error.ErrorMessage;
        //            Console.WriteLine(errorMessage);
        //        }
        //    }

        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
            {
                User _user = _context.users.FirstOrDefault(u => u.Login == login && u.Password == password);
                if (_user != null)
                {
                    var claims = new List<Claim>() { new Claim(ClaimTypes.Name, login) };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    return RedirectToAction("Privacy", "Home");
                }
            }

            return Unauthorized();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize] 
        public IActionResult Profile()
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            var login = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            User _user = _context.users.FirstOrDefault(u => u.Login == login);

            return View(_user);
        }
    }
}
