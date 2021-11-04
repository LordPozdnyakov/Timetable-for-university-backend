using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;

using timetable.Data;
using timetable.Helpers;
using timetable.Models;


namespace timetable.Services
{
    public interface IUserService
    {
        // User-Authentication
        User Authenticate(string username, string password); // NotImplemented

        // CRUD-Methods
        User Create(User user/*, string password*/);
        List<User> GetAll();
        User GetById(int id);
        void Update(User user, string password = null); // NotImplemented
        void Delete(int id); // NotImplemented
    }

    public class UserService : IUserService
    {
        private DataContext _context;
        private readonly IEmailService _emailService;
        private readonly IPasswordService _passwordService;

        public UserService(DataContext context,
            IEmailService emailService,
            IPasswordService passwordService)
        {
            _context = context;
            _emailService = emailService;
            _passwordService = passwordService;
        }

        // User-Authentication
        // Maybe Unnecessary
        public User Authenticate(string username, string password)
        {
            throw new NotImplementedException();
            /*if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Email == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash))
                return null;

            // authentication successful
            return user;*/
        }

        // CRUD-Methods
        public User Create( User user/*, string password*/ )
        {
            // validation
            // if (string.IsNullOrWhiteSpace(password))
                // throw new AppException("Password is required");

            if (_context.Users.Any(x => x.Email == user.Email))
                throw new AppException("Email \"" + user.Email + "\" is already taken");
            
            user.isPasswordSet = false;

            // byte[] passwordHash, passwordSalt;
            // PasswordHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            // user.PasswordHash = passwordHash;
            // user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public User GetById(int id)
        {
            return _context.Users.FirstOrDefault(aac => aac.Id == id );
        }

        public void Update(User user, string password = null)
        {
            throw new NotImplementedException();

            /*var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            // update username if it has changed
            if (!string.IsNullOrWhiteSpace(userParam.Username) && userParam.Username != user.Username)
            {
                // throw error if the new username is already taken
                if (_context.Users.Any(x => x.Username == userParam.Username))
                    throw new AppException("Username " + userParam.Username + " is already taken");

                user.Username = userParam.Username;
            }

            // update user properties if provided
            if (!string.IsNullOrWhiteSpace(userParam.FirstName))
                user.FirstName = userParam.FirstName;

            if (!string.IsNullOrWhiteSpace(userParam.LastName))
                user.LastName = userParam.LastName;

            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();*/
        }
        
        public void Delete(int id)
        {
            throw new NotImplementedException();

            /*var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }*/
        }
    }
}