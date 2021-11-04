using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

using timetable.Configuration;
using timetable.Models;
using timetable.Data;
using timetable.Helpers;


namespace timetable.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("/login")]

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
        public async Task<ActionResult<LoginResponse>> PostLogin([FromBody] LoginRequest model)
        {
            Console.Write("Post_Login\n");
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            var user_by_login = await _context.Users.FirstOrDefaultAsync(aac => aac.Email == model.Email);
            
            // User not found
            if( user_by_login == null )
            {
                return Unauthorized();
            }

            // Check Password
            if( PasswordHelper.VerifyPasswordHash(
                    model.Password,
                    user_by_login.PasswordHash,
                    user_by_login.PasswordSalt
                ))
            {
                return Unauthorized();
            }

            // Create Token
            var tokenHelper = new TokenHelper(_appSettings);
            var tokenString = tokenHelper.GenerateToken(user_by_login.Id, model.RememberMe);

            _ = user_by_login.RememberMe = model.RememberMe;

            Response.Headers.Add("Token", tokenString);
            LoginResponse login = (LoginResponse)user_by_login;

            return login;
        }
    }
}