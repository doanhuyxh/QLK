using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.QuanLyThanhPhamViewModel
{
    public class QuanLyThanhPhamCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Tên thành phẩm")]
        [Required(ErrorMessage = "Tên thành phẩm không được để trống")]
        public string TenThanhPham { get; set; }

        [Display(Name = "Miêu tả")]
        public string GhiChu { get; set; }

        [Display(Name = "Số Lượng")]
        public int SoLuong { get; set; }

        [Display(Name = "Khách hàng")]
        public string KhachHang { get; set; }

        [Display(Name = "Màu sắc")]
        public string Mau { get; set; }

        [Display(Name = "Size")]
        public string Size { get; set; }

        [Display(Name = "PO")]
        public string PO { get; set; }

        public static implicit operator QuanLyThanhPhamCRUDViewModel(QuanLyThanhPham _QuanLyThanhPham)
        {
            return new QuanLyThanhPhamCRUDViewModel
            {
                Id = _QuanLyThanhPham.Id,
                TenThanhPham = _QuanLyThanhPham.TenThanhPham,
                GhiChu = _QuanLyThanhPham.GhiChu,
                SoLuong = _QuanLyThanhPham.SoLuong,
                CreatedDate = _QuanLyThanhPham.CreatedDate,
                ModifiedDate = _QuanLyThanhPham.ModifiedDate,
                CreatedBy = _QuanLyThanhPham.CreatedBy,
                ModifiedBy = _QuanLyThanhPham.ModifiedBy,
                Cancelled = _QuanLyThanhPham.Cancelled,
                KhachHang = _QuanLyThanhPham.KhachHang,
                Mau = _QuanLyThanhPham.Mau,
                Size = _QuanLyThanhPham.Size,
                PO = _QuanLyThanhPham.PO,
            };
        }

        public static implicit operator QuanLyThanhPham(QuanLyThanhPhamCRUDViewModel vm)
        {
            return new QuanLyThanhPham
            {
                Id = vm.Id,
                TenThanhPham = vm.TenThanhPham,
                GhiChu = vm.GhiChu,
                SoLuong = vm.SoLuong,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
                KhachHang = vm.KhachHang,
                Mau = vm.Mau,
                Size = vm.Size,
                PO = vm.PO,
            };
        }
    }
}
