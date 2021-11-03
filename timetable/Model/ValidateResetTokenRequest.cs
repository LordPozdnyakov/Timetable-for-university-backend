using System.ComponentModel.DataAnnotations;

namespace timetable.Model
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}