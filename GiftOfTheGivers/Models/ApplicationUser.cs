using Microsoft.AspNetCore.Identity;

namespace GiftOfTheGivers.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Role { get; set; } // Admin, User, Volunteer
    }
}
