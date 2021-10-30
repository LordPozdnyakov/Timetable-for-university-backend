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
            result.SurName = that.SurName;
            result.GroupName = that.GroupName;
            
            return result;
        }

        // Privilege
        public string Privilege { get; set; }

        // Name
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SurName { get; set; }

        // Others
        public string GroupName { get; set; }
    }
}