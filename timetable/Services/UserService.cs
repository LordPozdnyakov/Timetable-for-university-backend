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

        // Helpers-Method for Password
        void CreateInvitationPassword(User user, string origin);
        void ForgotPassword(RecoveryByEmail model, string origin);
        void ResetPassword(ResetPasswordRequest model);
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

        // User-Authentication
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


        // Public Helpers-Method for Password
        public void CreateInvitationPassword(User user, string origin)
        {
            // check if email exist
            var account = _context.Users.SingleOrDefault(x =>
                x.Email == user.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return;

            // create reset token that expires after 1 day
            account.ResetToken = randomTokenString();
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(20);

            _context.Users.Update(account);
            _context.SaveChanges();
            bool create = true;

            // send email
            sendPasswordResetEmail(account, origin, create);
        }

        public void ForgotPassword(RecoveryByEmail model, string origin)
        {
            // check if email exist
            var account = _context.Users.SingleOrDefault(x => 
                x.Email == model.Email);

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

        public void ResetPassword(ResetPasswordRequest model)
        {
            var account = _context.Users.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
                throw new AppException("Invalid token");

            // Update password
            byte[] passwordHash, passwordSalt;
            PasswordHelper.CreatePasswordHash(model.Password, out passwordHash, out passwordSalt);
            account.PasswordHash = passwordHash;
            account.PasswordSalt = passwordSalt;

            //  Remove reset-token
            account.PasswordReset = DateTime.UtcNow;
            account.ResetToken = null;
            account.ResetTokenExpires = null;

            _context.Users.Update(account);
            _context.SaveChanges();
        }


        // Private Helpers-Methods
        private string randomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);

            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
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
            else 
            {
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
    }
}