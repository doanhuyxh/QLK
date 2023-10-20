using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.NhomKhachHangViewModel
{
    public class NhomKhachHangCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Tên  Nhóm Khách Hàng Mới")]
        public string TenNhomKhachHang { get; set; }


        [Display(Name = "Ghi Chú Nhóm Mới")]
        public string GhiChu { get; set; }

        public static implicit operator NhomKhachHangCRUDViewModel(NhomKhachHang _NhomKhachHang)
        {
            return new NhomKhachHangCRUDViewModel
            {
                Id = _NhomKhachHang.Id,
                TenNhomKhachHang = _NhomKhachHang.TenNhomKhachHang,
                GhiChu = _NhomKhachHang.GhiChu,
                CreatedDate = _NhomKhachHang.CreatedDate,
                ModifiedDate = _NhomKhachHang.ModifiedDate,
                CreatedBy = _NhomKhachHang.CreatedBy,
                ModifiedBy = _NhomKhachHang.ModifiedBy,
                Cancelled = _NhomKhachHang.Cancelled,
            };
        }

        public static implicit operator NhomKhachHang(NhomKhachHangCRUDViewModel vm)
        {
            return new NhomKhachHang
            {
                Id = vm.Id,
                TenNhomKhachHang = vm.TenNhomKhachHang,
                GhiChu = vm.GhiChu,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
