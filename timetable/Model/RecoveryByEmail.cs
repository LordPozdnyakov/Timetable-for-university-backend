using System.ComponentModel.DataAnnotations;


namespace timetable.Models
{
    public class RecoveryByEmail
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}