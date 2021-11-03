using System.ComponentModel.DataAnnotations;


namespace timetable.Models
{
    public class Login
    {
        public static explicit operator Login(User that)
        {
            Login result = new Login();
            
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