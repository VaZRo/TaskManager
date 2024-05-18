using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Contexts
{
    public class UserContext : DbContext
    {
        public DbSet<Models.User> users { get; set; }
        public DbSet<Models.TaskModel> tasks { get; set; }
        public DbSet<Models.Group> groups { get; set; }
        public UserContext(DbContextOptions<UserContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }

        
    }
}
