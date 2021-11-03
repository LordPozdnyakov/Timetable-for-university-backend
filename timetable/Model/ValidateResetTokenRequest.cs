using System.ComponentModel.DataAnnotations;

namespace timetable.Models
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}