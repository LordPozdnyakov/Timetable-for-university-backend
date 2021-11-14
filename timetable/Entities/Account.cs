using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using timetable.Entities;

namespace WebApi.Entities
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        // Login
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool isPasswordSet { get; set; }
        public bool RememberMe { get; set; }
        public string Token { get; set; }


        // Password Service
        public string ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public string code { get; set; }
        public DateTime PasswordReset { get; internal set; }

        // Privilege
        public string Role { get; set; }
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
       
        public string VerificationToken { get; set; }
       // public DateTime? Verified { get; set; }
        //public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;
       
       

        
    }
}