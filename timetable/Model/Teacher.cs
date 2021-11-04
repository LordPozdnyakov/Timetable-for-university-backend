using System.ComponentModel.DataAnnotations;


namespace timetable.Models
{
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }

        // Login
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool RememberMe { get; set; }
        public string Token { get; set; }

        // Privilege
        public string Privilege { get; set; }

        // Name
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }

        // Others
        public string PhoneNumber { get; set; }
    }
}