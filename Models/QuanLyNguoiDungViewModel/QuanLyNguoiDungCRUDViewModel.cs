using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.QuanLyNguoiDungViewModel
{
    public class QuanLyNguoiDungCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Họ và tên")]
        public string HoVaTen { get; set; }


        [Display(Name = "Ảnh đại diện")]
        public string? AnhDaiDienPath { get; set; }


        [Display(Name = "Ảnh đại diện")]
        public IFormFile AnhDaiDien { get; set; }


        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }


        [Display(Name = "Tài khoản")]
        public string TaiKhoan { get; set; }


        [Display(Name = "Nhóm người dùng")]
        public string? QuanLyNhomNguoiDungDisplay { get; set; }


        [Display(Name = "Nhóm người dùng")]
        public int QuanLyNhomNguoiDungId { get; set; }


        [Display(Name = "Ngày thêm")]
        public DateTime NgayThem { get; set; }


        [Display(Name = "Chức vụ")]
        public string ChucVu { get; set; }

        public static implicit operator QuanLyNguoiDungCRUDViewModel(QuanLyNguoiDung _QuanLyNguoiDung)
        {
            return new QuanLyNguoiDungCRUDViewModel
            {
                Id = _QuanLyNguoiDung.Id,
                HoVaTen = _QuanLyNguoiDung.HoVaTen,
                AnhDaiDienPath = _QuanLyNguoiDung.AnhDaiDienPath,
                SoDienThoai = _QuanLyNguoiDung.SoDienThoai,
                TaiKhoan = _QuanLyNguoiDung.TaiKhoan,
                QuanLyNhomNguoiDungId = _QuanLyNguoiDung.QuanLyNhomNguoiDungId,
                NgayThem = _QuanLyNguoiDung.NgayThem,
                ChucVu = _QuanLyNguoiDung.ChucVu,
                CreatedDate = _QuanLyNguoiDung.CreatedDate,
                ModifiedDate = _QuanLyNguoiDung.ModifiedDate,
                CreatedBy = _QuanLyNguoiDung.CreatedBy,
                ModifiedBy = _QuanLyNguoiDung.ModifiedBy,
                Cancelled = _QuanLyNguoiDung.Cancelled,
            };
        }

        public static implicit operator QuanLyNguoiDung(QuanLyNguoiDungCRUDViewModel vm)
        {
            return new QuanLyNguoiDung
            {
                Id = vm.Id,
                HoVaTen = vm.HoVaTen,
                AnhDaiDienPath = vm.AnhDaiDienPath,
                SoDienThoai = vm.SoDienThoai,
                TaiKhoan = vm.TaiKhoan,
                QuanLyNhomNguoiDungId = vm.QuanLyNhomNguoiDungId,
                NgayThem = vm.NgayThem,
                ChucVu = vm.ChucVu,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
