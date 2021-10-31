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
        [Authorize]
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
        public async Task<ActionResult<Login>> PostLogin([FromBody] LoginRequest model)
        {
            Console.Write("Post_Login\n");
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var user_by_login = await _context.Users.FirstOrDefaultAsync(aac => aac.Email == model.Login);

            // User not found
            if (user_by_login == null)
            {
                return Unauthorized();
            }

            // Check Password
            /*if (user_by_login.PasswordHash != PasswordToHash(model.Password))
            {
                return Unauthorized();
            }*/

            // Create Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var expire = (model.RememberMe == true) ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(2);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user_by_login.UserId.ToString())
                }),
                Expires = expire,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _ = user_by_login.RememberMe = model.RememberMe;

            Response.Headers.Add("Token", tokenString);
            Login login = (Login)user_by_login;

            return login;
        }
        string PasswordToHash(string Password)
        {
            return Password;
        }

        //Create Token
        // string user_token = GenerateToken();
        //user.RememberMe = model.RememberMe;
        //string user_token = user.Email + user.PasswordHash + user.RememberMe.ToString();
        //user.Token = user_token;


    }

}
