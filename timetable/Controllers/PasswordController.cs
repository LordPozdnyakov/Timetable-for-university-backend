using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;


using timetable.Configuration;
using timetable.Data;
using timetable.Helpers;
using timetable.Models;
using timetable.Services;


namespace timetable.Controllers
{
    [ApiController]
    [AllowAnonymous]

    public class PasswordController : Controller
    {
        private DataContext _context;
        private readonly AppSettings _appSettings;
        private IUserService _userService;
        private IPasswordService _passwordService;
        public PasswordController(
            DataContext context, 
            IUserService userService,
            IPasswordService passwordService,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            _userService = userService;
            _passwordService = passwordService;
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        [Route("/reset-password")]
        public IActionResult ResetPassword(ResetPasswordRequest model)
        {
            _passwordService.ResetPassword(model);
            return Ok(new { message = "Password reset successful, you can now login" });
        }

        [HttpPost]
        [Route("/forgot-password")]
        public IActionResult ForgotPassword(RecoveryByEmail model)
        {
            _passwordService.ForgotPassword(model, Request.Headers["origin"]);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }
    }
}