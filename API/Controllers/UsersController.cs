using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
   
    //[ApiController]
    //[Route("api/[controller]")] // URL will be localhost:api/Users
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly DataContext _objDataContext;
        public UsersController(DataContext objDataContext)
        {
            _objDataContext = objDataContext;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            // gives all rows
            var users = await _objDataContext.Users.ToListAsync();
            return users;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            // gives specific user; id must be primary key
            var user = await _objDataContext.Users.FindAsync(id);
            return user;
        }
    }
}