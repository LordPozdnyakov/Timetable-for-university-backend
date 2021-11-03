using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;
using timetable.Data;
using timetable.Helpers;
using timetable.Model;
using timetable.Models;

namespace timetable.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(int id);
        Task GeneratePasswordResetTokenAsync(int userId);
        void ForgotPassword(EmailRecovery model, string origin);
        void ValidateResetToken(ValidateResetTokenRequest model);
        void ResetPassword(ResetPasswordRequest model);
        void CreatPassword(User user, string origin);
    }

    public class UserService : IUserService
    {
        private DataContext _context;
        private readonly IEmailService _emailService;

        public UserService(DataContext context,
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

       /* public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Email == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash))
                return null;

            // authentication successful
            return user;
        }*/

        private bool VerifyPasswordHash(string password, string passwordHash)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }
        /*public User ForgotPass(User user)
        {
            if (_context.Users.Any(x => x.Email == user.Email))
            {
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.UserId, code = code }, protocol: HttpContext.Request.Scheme);
            }

            return Email;
        }*/
        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.Email == user.Email))
                throw new AppException("Email \"" + user.Email + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        /*
        public void Update(User userParam, string password = null)
        {
            var user = _context.Users.Find(userParam.Id);

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
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }*/

        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

       

        public void Update(User user, string password = null)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public User Authenticate(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task GeneratePasswordResetTokenAsync(int userId)
        {
            throw new NotImplementedException();
        }

   
        private string randomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        } 
        public void ForgotPassword(EmailRecovery model, string origin)
        {
            var account = _context.Users.SingleOrDefault(x => x.Email == model.Email);
            // always return ok response to prevent email enumeration
            if (account == null) return;

            // create reset token that expires after 1 day
            account.ResetToken = randomTokenString();
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

            _context.Users.Update(account);
            _context.SaveChanges();
            bool create = false;

            // send email
            sendPasswordResetEmail(account, origin, create);
        }
        private void sendPasswordResetEmail(User account, string origin, bool create)
        {
            string message;
            if (create == false)
            {
                
                if (!string.IsNullOrEmpty(origin))
                {
                    var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
                    message = $@"<p>Please click the below link to reset your password, the link will be valid for 20 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
                }
                else
                {
                    message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                             <p><code>{account.ResetToken}</code></p>";
                }
            }
            else {
                var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
                message = $@"<p>To complete registrations follow the link, the link will be valid for 20 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Administrator from Timetable - Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                         {message}"
            );
        }
        public void ValidateResetToken(ValidateResetTokenRequest model)
        {
            var account = _context.Users.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
                throw new AppException("Invalid token");

        }
        public void ResetPassword(ResetPasswordRequest model)
        {
            var account = _context.Users.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
                throw new AppException("Invalid token");

            // update password and remove reset token
            //account.PasswordHash = BC.HashPassword(model.Password);
            account.PasswordReset = DateTime.UtcNow;
            account.ResetToken = null;
            account.ResetTokenExpires = null;

            _context.Users.Update(account);
            _context.SaveChanges();
        }


  

        public void CreatPassword(User user, string origin)
        {
            var account = _context.Users.SingleOrDefault(x => x.Email == user.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return;

            // create reset token that expires after 1 day
            account.ResetToken = randomTokenString();
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(20);

            _context.Users.Update(account);
            _context.SaveChanges();
            bool create = true;
            sendPasswordResetEmail(account, origin, create);
        }
    }
}