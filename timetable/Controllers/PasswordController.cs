using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using timetable.Configuration;


namespace timetable.Controllers
{
    public static class PasswordController
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            var hmac = new System.Security.Cryptography.HMACSHA512();

            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            return CmpBytes( computedHash, storedHash, computedHash.Length );
        }

        public static bool CmpBytes( byte[] inLeft, byte[] inRight, int inLenght )
        {
            for (int i = 0; i < inLenght; i++)
            {
                if (inLeft[i] != inRight[i]) return false;
            }

            return true;
        }
       
    }
}