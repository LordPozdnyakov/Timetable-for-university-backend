using BackendWeb.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendWeb.Controllers
{
    [Route("v1/users")]
    [ApiController]
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
        public async Task<ActionResult<Users>> Post([FromServices] DatasContext context, [FromBody] Users model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            context.User.Add(model);
            await context.SaveChangesAsync();
            return model;
        }

    }
}
