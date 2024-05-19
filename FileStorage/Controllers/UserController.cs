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
using System.Linq;

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
                else
                {
                    TempData["ErrorMessage"] = "User With This Login Already exist.";
                }
                
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
            {
                User _user = _context.users.FirstOrDefault(u => u.Login == login && u.Password == password);
                if (_user != null)
                {
                    var claims = new List<Claim>() {
                        new Claim(ClaimTypes.Name, login),
                        new Claim(ClaimTypes.NameIdentifier, _user.Id.ToString()),
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    return RedirectToAction("Privacy", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "User With This Login Doesn't exist.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Login and password are required.";
            }

            return View();
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
            var finishedTaskCount = _context.tasks
            .Join(_context.groups,
                  t => t.GroupId,
                  g => g.Id,
                  (t, g) => new { Task = t, Group = g })
            .Where(tg => tg.Group.UserId == _user.Id && tg.Task.CompletedOn == 100)
            .Count();

            var allTaskCount = _context.tasks
            .Join(_context.groups,
                  t => t.GroupId,
                  g => g.Id,
                  (t, g) => new { Task = t, Group = g })
            .Where(tg => tg.Group.UserId == _user.Id)
            .Count();

            int groupsCount = _context.groups.Where(g => g.UserId == _user.Id).Count();

            ViewBag.GroupsCount = groupsCount;
            ViewBag.TaskCount = allTaskCount;
            ViewBag.FinishedTasks = finishedTaskCount;

            return View(_user);
        }
    }
}
