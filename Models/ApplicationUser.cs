using Microsoft.AspNetCore.Identity;

namespace AMS.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int GroupUserId { get; set; }
    }
}
