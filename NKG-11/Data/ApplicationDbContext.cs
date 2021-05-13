using NGK_11.Models;
using Microsoft.EntityFrameworkCore;

namespace NGK_11.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Measurement> Measurements { get; set; }

        public DbSet<User> Users { get; set; }
    }

}
