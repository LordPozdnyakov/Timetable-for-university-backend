using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

using timetable.Models;
using timetable.Data;

namespace timetable.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/users")]

    public class UserController : Controller
    {
        private DataContext _context;

        public UserController( DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers([FromServices] DataContext context)
        {
            var users = await context.Users.ToListAsync();
            return users;
        }
        
        [HttpGet]
        [Route("/{id}")]
        public async Task<ActionResult<User>> GetUsersById([FromServices] DataContext context, [FromRoute]long id)
        {
            var userItem = await _context.Users.FirstOrDefaultAsync(aac => aac.UserId == id );

            if (userItem == null)
            {
                return NotFound();
            }

            return userItem;
        }
        
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<User>> PostUser([FromServices] DataContext context, [FromBody] User model)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            context.Users.Add(model);
            await context.SaveChangesAsync();
            return model;
        }
    }
}