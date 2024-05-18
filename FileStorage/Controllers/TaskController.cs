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


        public IActionResult TasksPage(int group)
        {
            Models.Group groupModel = _context.groups.FirstOrDefault(g => g.Id == group);
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            int userId = Int32.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if(groupModel == null)
            {
                return NotFound();
            }

            if(userId == groupModel.UserId)
            {
                IQueryable<TaskModel> tasks = _context.tasks.Where(t => t.GroupId == group);
                return View(tasks.ToList());
            }

            return NotFound();
        }

        public IActionResult AddTask()
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

        [HttpPost]
        public async Task<IActionResult> AddTask(Models.TaskModel task, IFormFile avatarFile)
        {
            Models.Group groupModel = _context.groups.FirstOrDefault(g => g.Id == task.GroupId);
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            int userId = Int32.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if(groupModel.UserId != userId)
            {
                return NotFound();
            }


            if(avatarFile != null && avatarFile.Length > 0)
            {
                using(var memoryStream = new MemoryStream())
                {
                    await avatarFile.CopyToAsync(memoryStream);
                    task.Avatar = memoryStream.ToArray();
                }
            }

            if(task.DeadLine.Date >= DateTime.UtcNow)
            {
                task.CompletedOn = 0;
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction("TasksPage", "Task", new { group = task.GroupId });
            }
            else
            {
                TempData["ErrorMessage"] = "Deadline cannot be set in the past.";
            }
            return View();
        }


    }
}
