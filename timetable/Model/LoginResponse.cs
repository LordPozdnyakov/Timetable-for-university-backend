using System.ComponentModel.DataAnnotations;


namespace timetable.Models
{
    public class LoginResponse
    {
        public static explicit operator LoginResponse(User that)
        {
            LoginResponse result = new LoginResponse();
            
            result.Privilege = that.Privilege;
            result.FirstName = that.FirstName;
            result.LastName = that.LastName;
            result.Patronymic = that.Patronymic;
            result.GroupName = that.GroupName;
            
            return result;
        }

        // Token
        [Required]
        public string Token { get; set; }

        // Privilege
        [Required]
        public string Privilege { get; set; }

        // Name
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Patronymic { get; set; }

        // Others
        public string GroupName { get; set; }
    }
}