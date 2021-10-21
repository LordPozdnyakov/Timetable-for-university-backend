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
    [Route("v1/users")]

    public class UserController : Controller
    {
        private DataContext _context;

        public UserController( DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<User>>> GetAction([FromServices] DataContext context)
        {
            var users = await context.Users.ToListAsync();
            return users;
        }
        
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<User>> Post([FromServices] DataContext context, [FromBody] User model)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            context.Users.Add(model);
            await context.SaveChangesAsync();
            return model;
        }
    }
}