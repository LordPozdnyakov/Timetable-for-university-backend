using System;
using System.ComponentModel.DataAnnotations;

namespace timetable.Models
{
    public class User
    {
    // MERGED
        public string ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public string code { get; set; }
        public DateTime PasswordReset { get; internal set; }
    // / MERGED

        [Key]
        public int Id { get; set; }

        // Login
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool RememberMe { get; set; }
        public string Token { get; set; }

        // Privilege
        public string Privilege { get; set; }

        // Name
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }

        // Parents
        public string FatherName { get; set; }
        public string FatherPhone { get; set; }
        public string MotherName { get; set; }
        public string MotherPhone { get; set; }

        // Others
        public string BirthDay { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string GroupName { get; set; }
    }
}