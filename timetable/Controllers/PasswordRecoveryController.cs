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
    [Route("/password_recovery")]

    public class PasswordRecoveryController: Controller
    {
        private DataContext _context;
        public PasswordRecoveryController( DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<StatusCodeResult>> SetNewPassword(
            [FromServices] DataContext context,
            [FromBody] PasswordRecovery model,
            [FromQuery] ValidateResetTokenRequest token
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

                return new StatusCodeResult(200);
            }
            return new StatusCodeResult(407);
        }
    }
}