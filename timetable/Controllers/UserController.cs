using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using timetable.Models;
using timetable.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using timetable.Model;
using timetable.Services;
using AutoMapper;
using timetable.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace timetable.Controllers
{
    [ApiController]
    // [Route("users")]

    public class UserController : Controller
    {
        private DataContext _context;
        private readonly AppSettings _appSettings;
        private IUserService _userService;
        private IMapper _mapper;
        
        public UserController( DataContext context, 
            //IOptions<AppSettings> appSettings,
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            //_appSettings = appSettings.Value;
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        [Route("users")]
        public async Task<ActionResult<List<User>>> GetUsers([FromServices] DataContext context)
        {
            Console.Write("Get_Users\n");
            var users = await context.Users.ToListAsync();
            return users;
        }
        //[AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            // map model to entity
            var user = _mapper.Map<User>(model);
           
            try
            {
                // create user
                _userService.Create(user, model.Password);
                EmailService emailService = new EmailService();
                await emailService.SendEmailAsync(model.Email, "Registration on project", $"Login:{model.Email}\\Password:{model.Password}");
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
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
            if( user == null || user.Password != model.Password)
            {
                // user not found
                return Unauthorized();
            }

            // Check Password
            // if( user.PasswordHash != ToHash(model.PasswordHash) )
           if (model.RememberMe == false){
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddSeconds(2),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                //model.Token = token;
                _ = user.RememberMe = false;
                _ = user.Token = tokenString;

                return user;

            }
           else
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                _ = user.RememberMe = true;
                _ = user.Token = tokenString;
                //model.Token = token
                return user;
            }

            //Create Token
            // string user_token = GenerateToken();
            //user.RememberMe = model.RememberMe;
            //string user_token = user.Email + user.PasswordHash + user.RememberMe.ToString();
            //user.Token = user_token;

            
        }
    }
}