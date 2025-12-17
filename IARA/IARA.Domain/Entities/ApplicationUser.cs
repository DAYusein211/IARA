using Microsoft.AspNetCore.Identity;

namespace IARA.Domain.Entities
{
    // ApplicationUser extends IdentityUser to allow extension in the future (first/last name, etc.)
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

