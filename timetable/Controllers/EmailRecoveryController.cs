using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

using timetable.Configuration;
using timetable.Models;
using timetable.Data;


namespace timetable.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("emailrecovery")]

    public class EmailRecoveryController : Controller
    {
        private DataContext _context;
        public EmailRecoveryController( DataContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        public async Task<ActionResult<StatusCodeResult>> SendRecoveryOnEmail([FromBody] EmailRecovery model)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            var user_by_login = await _context.Users.FirstOrDefaultAsync(aac => aac.Email == model.Email);
            
            // User not found
            if( user_by_login == null )
            {
                return new StatusCodeResult(407);
            }

            // Generate CODE
            // Store CODE with email
            // Send CODE on email
            
            return new StatusCodeResult(200);
        }
    }
}