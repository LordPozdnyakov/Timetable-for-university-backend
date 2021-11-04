using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

using timetable.Controllers;
using timetable.Configuration;
using timetable.Data;
using timetable.Helpers;
using timetable.Models;


namespace timetable.Services
{
    public interface IPasswordService
    {
        // Helpers-Method for Password
        void CreateInvitationPassword(User user, string origin);
        void ForgotPassword(RecoveryByEmail model, string origin);
        void ResetPassword(ResetPasswordRequest model);
    }

    public class PasswordService : IPasswordService
    {
        private DataContext _context;
        private readonly IEmailService _emailService;

        public PasswordService(DataContext context,
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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

