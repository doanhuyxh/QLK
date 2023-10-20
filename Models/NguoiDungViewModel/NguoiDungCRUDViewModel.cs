using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.NguoiDungViewModel
{
    public class NguoiDungCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display (Name = "Họ và tên")]
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        public string HoVaTen { get; set; }


        [Display (Name = "Tài khoản")]
        [Required(ErrorMessage ="Tài khoản không được để trống")]
        public string Username { get; set; }


        [Display (Name = "Mật khẩu")]
        [Required(ErrorMessage ="Mật khẩu không được để trống")]
        public string Password { get; set; }


        [Display (Name = "Địa chỉ")]
        [Required(ErrorMessage ="Địa chỉ không được để trống")]

        public string DiaChi { get; set; }


        [Display(Name = "Nhóm người dùng")]
        public string? NhomNguoiDungDisplay { get; set; }


        [Display(Name = "Nhóm người dùng")]
        [Required(ErrorMessage ="Chọn nhóm người dùng")]
        public int NhomNguoiDungId { get; set; }


        [Display(Name = "Là Supper Admin")]
        public bool IsSupperAdmin { get; set; }

        public static implicit operator NguoiDungCRUDViewModel(NguoiDung _NguoiDung)
        {
            return new NguoiDungCRUDViewModel
            {
                Id = _NguoiDung.Id,
                HoVaTen = _NguoiDung.HoVaTen,
                Username = _NguoiDung.Username,
                Password = _NguoiDung.Password,
                DiaChi = _NguoiDung.DiaChi,
                NhomNguoiDungId = _NguoiDung.NhomNguoiDungId,
                IsSupperAdmin = _NguoiDung.IsSupperAdmin,
                CreatedDate = _NguoiDung.CreatedDate,
                ModifiedDate = _NguoiDung.ModifiedDate,
                CreatedBy = _NguoiDung.CreatedBy,
                ModifiedBy = _NguoiDung.ModifiedBy,
                Cancelled = _NguoiDung.Cancelled,
            };
        }

        public static implicit operator NguoiDung(NguoiDungCRUDViewModel vm)
        {
            return new NguoiDung
            {
                Id = vm.Id,
                HoVaTen = vm.HoVaTen,
                Username = vm.Username,
                Password = vm.Password,
                DiaChi = vm.DiaChi,
                NhomNguoiDungId = vm.NhomNguoiDungId,
                IsSupperAdmin = vm.IsSupperAdmin,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
