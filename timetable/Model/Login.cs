using System.ComponentModel.DataAnnotations;

namespace timetable.Models
{
    public class Login
    {
        [Key]
        public int LoginId { get; set; }

        [MaxLength(1024, ErrorMessage = "Max Lenght ofname is 1024 symbols")]
        public string Email { get; set; }

        public string Password { get; set; }

        public bool RemeberMe { get; set; }
    }
}