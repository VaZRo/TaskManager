using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class Task
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string CompletedOn { get; set; }
        [Required]
        public DateTime DeadLine { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public byte[]? Avatar { get; set; }
    }
}
