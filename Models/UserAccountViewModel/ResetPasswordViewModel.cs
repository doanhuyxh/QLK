using System.ComponentModel.DataAnnotations;

namespace AMS.Models.UserAccountViewModel
{
    public class ResetPasswordViewModel
    {
        public string ApplicationUserId { get; set; }

        [Required]
        [Display(Name = "Mật khẩu cũ")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} phải dài ít nhất {2} và tối đa {1} ký tự.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}
