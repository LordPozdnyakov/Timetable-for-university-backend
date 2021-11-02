using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using timetable.Configuration;
using timetable.Models;
using timetable.Data;


namespace timetable.Controllers
{
    [ApiController]

    public class PasswordRecoveryController: Controller
    {
        private DataContext _context;
        public PasswordRecoveryController( DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("/password_recovery")]
        public async Task<StatusCodeResult> SetNewPassword(
            [FromServices] DataContext context,
            [FromBody] PasswordRecovery model,
            [FromQuery] TokenCode token
            )
        {
            if( model.Password == model.PasswordRepeat )
            {
                // var userItem = await _context.Users.FirstOrDefaultAsync(aac => aac.token == token.Token );
                var userItem = await _context.Users.FirstOrDefaultAsync();
                
                byte[] passwordHash, passwordSalt;
                PasswordController.CreatePasswordHash(model.Password, out passwordHash, out passwordSalt);

                userItem.PasswordHash = passwordHash;
                userItem.PasswordSalt = passwordSalt;

                return Ok();
            }
            return BadRequest();
        }
    }
}