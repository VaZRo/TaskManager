using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class Group
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public byte[]? Avatar { get; set; }
    }
}
