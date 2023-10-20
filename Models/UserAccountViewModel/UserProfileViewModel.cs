using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace AMS.Models.UserAccountViewModel
{
    public class UserProfileViewModel : EntityBase
    {
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
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Quốc gia")]
        public string Country { get; set; }
        [Display(Name = "Nhóm người dùng")]
        public int GroupUserId { get; set; }
        [Display(Name = "Nhóm người dùng")]
        public string GroupUserDisplay { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required]
        [StringLength(100, ErrorMessage = "{0} phải dài ít nhất {2} và tối đa {1} ký tự.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        [Display(Name = "Xác nhận mật khẩu")]
        [Required]
        [DataType(DataType.Password)]
        [Compare("PasswordHash", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Mật khẩu cũ")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Display(Name = "Ảnh đại diện")]
        public IFormFile ProfilePictureDetails { get; set; }
        public IList<IFormFile> _IListFile { get; set; }
        public string ProfilePicture { get; set; } = "/upload/blank-person.png";
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CurrentURL { get; set; }


        public static implicit operator UserProfileViewModel(UserProfile _UserProfile)
        {
            return new UserProfileViewModel
            {
                UserProfileId = (int)_UserProfile.UserProfileId,
                ApplicationUserId = _UserProfile.ApplicationUserId,
                FirstName = _UserProfile.FirstName,
                LastName = _UserProfile.LastName,
                PhoneNumber = _UserProfile.PhoneNumber,
                Email = _UserProfile.Email,
                Address = _UserProfile.Address,
                Country = _UserProfile.Country,
                GroupUserId = _UserProfile.GroupUserId,

                PasswordHash = _UserProfile.PasswordHash,
                ConfirmPassword = _UserProfile.ConfirmPassword,
                OldPassword = _UserProfile.OldPassword,
                CreatedDate = _UserProfile.CreatedDate,
                ModifiedDate = _UserProfile.ModifiedDate,
                CreatedBy = _UserProfile.CreatedBy,
                ModifiedBy = _UserProfile.ModifiedBy
                //ProfilePicture = _UserProfile.ProfilePicture
            };
        }

        public static implicit operator UserProfile(UserProfileViewModel vm)
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
                GroupUserId = vm.GroupUserId,

                PasswordHash = vm.PasswordHash,
                ConfirmPassword = vm.ConfirmPassword,
                OldPassword = vm.OldPassword,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy
                //ProfilePicture = vm.ProfilePicture
            };
        }
    }
}
