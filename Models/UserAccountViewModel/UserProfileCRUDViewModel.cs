using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace AMS.Models.UserAccountViewModel
{
    public class UserProfileCRUDViewModel : EntityBase
    {
        [Display(Name = "User Profile")]
        public int UserProfileId { get; set; }

        public string ApplicationUserId { get; set; }

        [Display(Name = "Họ")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Tên")]
        [Required]
        public string LastName { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Display(Name = "Nhóm người dùng")]
        public int? GroupUserId { get; set; }
        [Display(Name = "Nhóm người dùng")]
        public string GroupUserDisplay { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Quốc gia")]
        public string Country { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public IFormFile ProfilePicture { get; set; }
        [Display(Name = "Ảnh đại diện")]
        public string ProfilePicturePath { get; set; }


        public static implicit operator UserProfileCRUDViewModel(UserProfile _UserProfile)
        {
            return new UserProfileCRUDViewModel
            {
                UserProfileId = (int)_UserProfile.UserProfileId,
                ApplicationUserId = _UserProfile.ApplicationUserId,
                FirstName = _UserProfile.FirstName,
                LastName = _UserProfile.LastName,
                PhoneNumber = _UserProfile.PhoneNumber,
                Email = _UserProfile.Email,
                Address = _UserProfile.Address,
                Country = _UserProfile.Country,

                CreatedDate = _UserProfile.CreatedDate,
                ModifiedDate = _UserProfile.ModifiedDate,
                CreatedBy = _UserProfile.CreatedBy,
                ModifiedBy = _UserProfile.ModifiedBy,
                ProfilePicturePath = _UserProfile.ProfilePicture
            };
        }

        public static implicit operator UserProfile(UserProfileCRUDViewModel vm)
        {
            return new UserProfile
            {
                UserProfileId = vm.UserProfileId,
                ApplicationUserId = vm.ApplicationUserId,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                PhoneNumber = vm.PhoneNumber,
                Email = vm.Email,
                Address = vm.Address,
                Country = vm.Country,

                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                ProfilePicture = vm.ProfilePicturePath
            };
        }
    }
}
