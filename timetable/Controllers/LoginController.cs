using System;
using System.Threading;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using timetable.Models;
using timetable.Data;

namespace timetable.Controllers
{
    [ApiController]
    [Route("login")]

    public class LoginController : Controller
    {
        private DataContext _context;
        private readonly AppSettings _appSettings;
        public LoginController( DataContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }
        
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Login>> PostLogin([FromBody] LoginRequest model)
        {
            Console.Write("Post_Login\n");
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            var user_by_login = await _context.Users.FirstOrDefaultAsync(aac => aac.Email == model.Login);
            
            // User not found
            if( user_by_login == null )
            {
                return Unauthorized();
            }

            // Check Password
            if( user_by_login.PasswordHash != PasswordToHash(model.Password) )
            {
                return Unauthorized();
            }

            // Create Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes( _appSettings.Secret );
            var expire = (model.RememberMe == true) ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(2);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user_by_login.UserId.ToString())
                }),
                Expires = expire,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _ = user_by_login.RememberMe = model.RememberMe;

            Response.Headers.Add("Token", tokenString);
            Login login = (Login)user_by_login;

            return login;
        }

        string PasswordToHash( string Password )
        {
            return Password;
        }
    }
}