using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using timetable.Model;
using timetable.Services;
using System.Configuration;
using Microsoft.AspNetCore.Identity;
using timetable.Models;
using AutoMapper;
using timetable.Data;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

/*
namespace timetable.Controllers
{
    [ApiController]
    public class ResetPassword : Controller
    {
        private DataContext _context;
        private readonly AppSettings _appSettings;
        private IUserService _userService;
        private IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public ResetPassword(DataContext context,
            //IOptions<AppSettings> appSettings,
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            UserManager<User> userManager)
        {
            _context = context;
            //_appSettings = appSettings.Value;
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _userManager = userManager;
        }

        // public IActionResult Index() => View(_userManager.Users.ToList());

        // [HttpGet]
        // [Route("ForgotPasword")]
        // [AllowAnonymous]
        // public IActionResult ForgotPassword()
        // {
        //     return Ok();
        // }

        [HttpPost]
        [Route("ForgotPasword")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            //var user = await _userManager.FindByEmailAsync(model.Email);
            
            var code = await .GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.UserId, code = code }, protocol: HttpContext.Request.Scheme);
            return Ok(new
                {
                    Token = callbackUrl
                }
            );
        }
    }
}*/
