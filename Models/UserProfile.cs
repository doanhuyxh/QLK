using System;

namespace AMS.Models
{
    public class UserProfile : EntityBase
    {
        public int UserProfileId { get; set; }
        public string ApplicationUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public int GroupUserId { get; set; }
        public string PasswordHash { get; set; }
        public string ConfirmPassword { get; set; }
        public string? OldPassword { get; set; }
        public string ProfilePicture { get; set; } = "/upload/blank-person.png";
    }
}
