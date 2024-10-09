using Microsoft.EntityFrameworkCore;

namespace CarInsurance.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Insuree> Insurees { get; set; }
    }
}
