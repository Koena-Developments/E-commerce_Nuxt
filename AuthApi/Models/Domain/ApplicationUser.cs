using Microsoft.AspNetCore.Identity;

namespace AuthApi.Models.Domain
{
    public class ApplicationUser: IdentityUser
    {
        public string? Name { get; set; }
        
    }    
}