using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public byte[]? Avatar { get; set; }

    }
}
