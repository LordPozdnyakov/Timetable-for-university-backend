using System.ComponentModel.DataAnnotations;

namespace timetable.Model 
{ 
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}