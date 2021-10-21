using System.ComponentModel.DataAnnotations;

namespace timetable.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(1024, ErrorMessage = "Max Lenght ofname is 1024 symbols")]
        public string Name { get; set; }

        public string Privilege { get; set; }

        public string PasswordHash { get; set; }
    }
}