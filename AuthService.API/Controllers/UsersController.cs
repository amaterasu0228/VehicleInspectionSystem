using AuthService.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _context.Users
                .Select(x => new
                {
                    x.Id,
                    x.Email,
                    x.Role
                })
                .ToList();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _context.Users
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Email,
                    x.Role
                })
                .FirstOrDefault();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("{id}/role")]
        public IActionResult UpdateRole(int id, [FromBody] string role)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            user.Role = role;
            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok();
        }
    }
}