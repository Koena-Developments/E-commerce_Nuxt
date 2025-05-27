using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Data
{
    // ApplicationDbContext inherits from IdentityDbContext<IdentityUser>
    // This provides all the necessary tables for Identity (Users, Roles, Claims, etc.)
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        // Constructor that accepts DbContextOptions, which is used by the DI container
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
       
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        }
    }
}
