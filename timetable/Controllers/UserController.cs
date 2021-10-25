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
    // [Route("users")]

    public class UserController : Controller
    {
        private DataContext _context;

        public UserController( DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("users")]
        public async Task<ActionResult<List<User>>> GetUsers([FromServices] DataContext context)
        {
            Console.Write("Get_Users\n");
            var users = await context.Users.ToListAsync();
            return users;
        }
        
        [HttpPost]
        [Route("users")]
        public async Task<ActionResult<User>> PostUser([FromServices] DataContext context, [FromBody] User model)
        {
            Console.Write("Post_Users\n");
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            context.Users.Add(model);
            await context.SaveChangesAsync();
            return model;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<User>> PostLogin([FromServices] DataContext context, [FromBody] User model)
        {
            Console.Write("Post_Login\n");
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            var user = await context.Users.FirstOrDefaultAsync(aac => aac.Email == model.Email);
            if( user == null )
            {
                // user not found
                return Unauthorized();
            }

            // Check Password
            // if( user.PasswordHash != ToHash(model.PasswordHash) )
            if( user.PasswordHash != model.PasswordHash )
            {
                // password is Wrong
                return Unauthorized();
            }

            //Create Token
            // string user_token = GenerateToken();
            user.RememberMe = model.RememberMe;
            string user_token = user.Email + user.PasswordHash + user.RememberMe.ToString();
            user.Token = user_token;

            return user;
        }
    }
}