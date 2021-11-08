using BackendWeb.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BackendWeb.Controllers
{
    [ApiController]
    [Route("v1/users")]
    public class UsersController : Controller
    {
        private DatasContext _context;
        public UsersController(DatasContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Users>>> GetAction([FromServices] DatasContext context)
        {
            var users = await context.User.ToListAsync();
            return users;
        }
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Users>> Post(Users user/*[FromServices] DatasContext context, [FromBody] Users model*/)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
            /*if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            context.User.Add(model);
            await context.SaveChangesAsync();
            return model;*/
        }

    }
}
