using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using timetable.Configuration;


namespace timetable.Controllers
{
    public class TokenController
    {
        public TokenController( AppSettings appSettings )
        {
            _key = Encoding.ASCII.GetBytes( appSettings.Secret );
        }
        static private byte[] _key { get; set; }

        public TokenValidationParameters GetTokenProperty()
        {
            var property = new TokenValidationParameters {
                // Check Issuer
                ValidateIssuer = false,
                // ValidIssuer = "",

                // Check Audience
                ValidateAudience = false,
                // ValidAudience = "",
                ValidateLifetime = true,

                // Set Security-Key
                IssuerSigningKey = new SymmetricSecurityKey(_key),
                ValidateIssuerSigningKey = true,

                ClockSkew = TimeSpan.Zero
            };
            return property;
        }

        public string GenerateToken(int inUserId, bool inRememberMe )
        {
            // Create Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var expire = (inRememberMe == true) ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddMinutes(30);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, inUserId.ToString())
                }),
                Expires = expire,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_key),
                    SecurityAlgorithms.HmacSha256Signature )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public bool VerifyToken( string token )
        {
            // Clear token from headers
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }
            
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var principal = jwtTokenHandler.ValidateToken(
                token, this.GetTokenProperty(), out var validatedToken
            );

            // Now we need to check if the token has a valid security algorithm
            if(validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase
                );

                if(result == false) return false;
            }

            // Will get the time stamp in unix time
            var utcExpiryDate = long.Parse(
                principal.Claims.FirstOrDefault( x => 
                    x.Type == JwtRegisteredClaimNames.Exp
                ).Value
            );

            // we convert the expiry date from seconds to the date
            var expDate = UnixTimeStampToDateTime(utcExpiryDate);
            if( expDate < DateTime.UtcNow )
            {
                return false;
            }

            return true;
        }

        protected static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dateTime;
        }
    }
}