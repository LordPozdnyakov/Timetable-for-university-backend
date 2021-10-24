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
    [Route("v1/login")]

    public class LoginController : Controller
    {
        private DataContext _context;

        public LoginController( DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Login>>> GetAction([FromServices] DataContext context)
        {
            var logins = await context.Logins.ToListAsync();
            return logins;
        }
        
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Login>> Post([FromServices] DataContext context, [FromBody] Login model)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            context.Logins.Add(model);
            await context.SaveChangesAsync();
            return model;
        }
    }
}