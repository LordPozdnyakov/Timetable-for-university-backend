using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using timetable.Models;
using timetable.Data;

namespace timetable.Controllers
{
    [ApiController]
    [Route("login")]

    public class LoginController : Controller
    {
        private DataContext _context;

        public LoginController( DataContext context)
        {
            _context = context;
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
            user_by_login.RememberMe = model.RememberMe;
            user_by_login.Token = GenerateToken();

            Response.Headers.Add("Token", "\""+user_by_login.Token+"\"");
            Login login = (Login)user_by_login;

            return login;
        }

        string PasswordToHash( string Password )
        {
            return Password;
        }

        string GenerateToken()
        {
            return "SuperHash";
        }
    }
}