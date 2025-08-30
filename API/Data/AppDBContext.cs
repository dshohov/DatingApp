using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<AppUser> Users {  get; set; }
    }
}
