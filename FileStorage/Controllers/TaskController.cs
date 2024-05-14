using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using TaskManager.Contexts;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly UserContext _context;

        public TaskController(UserContext context)
        {
            _context = context;
        }

        public IActionResult Group()
        {
            ClaimsIdentity claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            var id = Int32.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            IQueryable<Models.Group> groups = _context.groups.Where(g => g.UserId == id);

            return View(groups.ToList());
        }

        
        public IActionResult Task()
        {
            return View();
        }

        public IActionResult AddGroup() { 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddGroup(Models.Group group, IFormFile avatarFile)
        {
            if (avatarFile != null && avatarFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await avatarFile.CopyToAsync(memoryStream);
                    group.Avatar = memoryStream.ToArray();
                }
            }

            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = Int32.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            group.UserId = userId;

            _context.Add(group);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Group");
        }


    }
}
