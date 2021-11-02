using System.ComponentModel.DataAnnotations;

namespace timetable.Models
{
    public class TokenCode
    {
        [Required]
        public string Token { get; set; }
    }
}