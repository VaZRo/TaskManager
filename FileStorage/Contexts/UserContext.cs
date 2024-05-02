using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Contexts
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        public DbSet<User> users { get; set; }
    }
}
