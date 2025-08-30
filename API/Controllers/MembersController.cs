using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController(AppDBContext context) : Controller
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<AppUser>>> GetMembers()
        {
            var members = await context.Users.ToArrayAsync().ConfigureAwait(false);

            return members ;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetMember(string id)
        {
            var member = await context.Users.FindAsync(id).ConfigureAwait(false);
            if(member is null ) return NotFound();
            return member ;
        }

    }
}
