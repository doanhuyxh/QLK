using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.KhachHangViewModel
{
    public class KhachHangCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Họ và Tên")]
        public string HoVaTen { get; set; }

        [Display(Name = "Số Điện Thoại")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Địa Chỉ")]
        public string DiaChi { get; set; }

        [Display(Name = "Ghi Chú")]
        public string GhiChu { get; set; }
        [Display(Name = "Nhóm Khách Hàng")]
        public string? NhomKhachHangDisplay { get; set; }
        [Display(Name = "Nhóm Khách Hàng")]
        public int NhomKhachHangId { get; set; }

        public static implicit operator KhachHangCRUDViewModel(KhachHang _KhachHang)
        {
            return new KhachHangCRUDViewModel
            {
                Id = _KhachHang.Id,
                HoVaTen = _KhachHang.HoVaTen,
                SoDienThoai = _KhachHang.SoDienThoai,
                DiaChi = _KhachHang.DiaChi,
                GhiChu = _KhachHang.GhiChu,
                NhomKhachHangId = _KhachHang.NhomKhachHangId,
                CreatedDate = _KhachHang.CreatedDate,
                ModifiedDate = _KhachHang.ModifiedDate,
                CreatedBy = _KhachHang.CreatedBy,
                ModifiedBy = _KhachHang.ModifiedBy,
                Cancelled = _KhachHang.Cancelled,
            };
        }

        public static implicit operator KhachHang(KhachHangCRUDViewModel vm)
        {
            return new KhachHang
            {
                Id = vm.Id,
                HoVaTen = vm.HoVaTen,
                SoDienThoai = vm.SoDienThoai,
                DiaChi = vm.DiaChi,
                GhiChu = vm.GhiChu,
                NhomKhachHangId = vm.NhomKhachHangId,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
