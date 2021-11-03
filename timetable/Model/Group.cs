using System.ComponentModel.DataAnnotations;


namespace timetable.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }

        // Login
        [Required]
        public string GroupName { get; set; }
    }
}