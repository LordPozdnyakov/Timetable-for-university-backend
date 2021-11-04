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
        public List<User> GetUsers([FromServices] DataContext context)
        {
            var users = _userService.GetAll();
            return users;
        }

        [HttpGet]
        [Route("/{id}")]
        public ActionResult<User> GetUsersById([FromServices] DataContext context, [FromRoute]int id)
        {
            var userItem = _userService.GetById(id);
            if (userItem == null)
            {
                return NotFound();
            }

            return userItem;
        }

        [HttpPost]
        public ActionResult<User> PostUser([FromServices] DataContext context, [FromBody] User model)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            _userService.Create(model/*, ""*/);
            _userService.CreateInvitationPassword(model, Request.Headers["origin"]);

            return model;
        }
    }
}