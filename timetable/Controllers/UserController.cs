using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using timetable.Configuration;
using timetable.Data;
using timetable.Helpers;
using timetable.Models;
using timetable.Services;


namespace timetable.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/users")]

    public class UserController : Controller
    {
        private DataContext _context;
        private readonly AppSettings _appSettings;
        private IUserService _userService;
        private IMapper _mapper;

        public UserController( DataContext context, 
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
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
            var userItem = await _context.Users.FirstOrDefaultAsync(aac => aac.Id == id );

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

        // MERGED
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(EmailRecovery model)
        {
            _userService.ForgotPassword(model, Request.Headers["origin"]);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }

        [HttpPost("validate-reset-token")]
        public IActionResult ValidateResetToken(ValidateResetTokenRequest model)
        {
            _userService.ValidateResetToken(model);
            return Ok(new { message = "Token is valid" });
        }
        
        [HttpPost("reset-password")]
        public IActionResult ResetPassword(ResetPasswordRequest model)
        {
            _userService.ResetPassword(model);
            return Ok(new { message = "Password reset successful, you can now login" });
        }
        // / MERGED
    }
}