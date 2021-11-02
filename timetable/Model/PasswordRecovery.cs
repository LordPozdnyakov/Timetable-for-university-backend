using System.ComponentModel.DataAnnotations;

namespace timetable.Models
{
    public class PasswordRecovery
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PasswordRepeat { get; set; }
    }
}