using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Data
{
    // ApplicationDbContext inherits from IdentityDbContext to provide Identity features
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        // Constructor that accepts DbContextOptions and passes them to the base class
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // Override OnModelCreating to customize the model building process
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Call the base method to ensure Identity tables are configured
            base.OnModelCreating(builder);
        }
    }
}
