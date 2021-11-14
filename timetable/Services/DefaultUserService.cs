using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;

using timetable.Data;
using timetable.Entities;
using timetable.Helpers;
using timetable.Models;


namespace timetable.Services
{
    public interface IDefaultUserService
    {
        void CreateDefaultUser();
    }

    public class DefaultUserService : IDefaultUserService
    {
        private DataContext _context;

        public DefaultUserService(DataContext context)
        {
            _context = context;
        }

        public void CreateDefaultUser()
        {
            User user = new User();
            user.Email = "admin@example.com";
            user.FirstName = user.LastName = user.Privilege = "Admin";
            user.Role = Role.Admin;
            user.Privilege = "admin";

            user.isPasswordSet = true;

            var password = "admin123";
            byte[] passwordHash, passwordSalt;
            PasswordHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}