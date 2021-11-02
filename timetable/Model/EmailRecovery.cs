using System.ComponentModel.DataAnnotations;

namespace timetable.Models
{
    public class EmailRecovery
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}