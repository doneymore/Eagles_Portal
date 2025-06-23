using Microsoft.EntityFrameworkCore;

namespace Eagles_Portal.Models
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
           
        }

        public DbSet<Users> Users { get; set; }

       
    }
}
