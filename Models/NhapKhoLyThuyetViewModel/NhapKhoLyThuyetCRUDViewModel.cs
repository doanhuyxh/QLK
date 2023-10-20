using System.ComponentModel.DataAnnotations;
using AMS.Models;
using AMS.Models.NguyenLieuNhapKhoLyThuyetViewModel;

namespace AMS.Models.NhapKhoLyThuyetViewModel
{
    public class NhapKhoLyThuyetCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }
        [Display(Name ="Dự kiến ngày về")]
        public DateTime DuKienNgayVe { get;set;}
        [Display(Name = "Ngày Mua")]
        public DateTime NgayNhap { get; set; }
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }
        [Display(Name = "Mã Phiếu")]
        public string MaPhieu { get; set; } 
        [Display(Name = "Tên khách hàng")] 
        public string TenKhachHang { get; set; } 
        [Display(Name = "Mã Số Lô")]
        public string MaSoLo { get; set; }
        [Display(Name = "Trạng thái")]
        public bool Status { get; set; }
        [Display(Name = "Đơn vị tiền tệ")]
        public string DonViTienTe { get; set; }
        public bool IsDue { get; set; }

        public List<NguyenLieuNhapKhoLyThuyetCRUDViewModel> NguyenLieuNhapKhoLyThuyetList { get; set; }

        public static implicit operator NhapKhoLyThuyetCRUDViewModel(NhapKhoLyThuyet _NhapKhoLyThuyet)
        {
            return new NhapKhoLyThuyetCRUDViewModel
            {
                Id = _NhapKhoLyThuyet.Id,
                NgayNhap = _NhapKhoLyThuyet.NgayNhap,
                MoTa = _NhapKhoLyThuyet.MoTa,
                DuKienNgayVe = _NhapKhoLyThuyet.DuKienNgayVe,
                MaPhieu = _NhapKhoLyThuyet.MaPhieu,
                MaSoLo = _NhapKhoLyThuyet.MaSoLo,
                TenKhachHang = _NhapKhoLyThuyet.TenKhachHang,
                Status = _NhapKhoLyThuyet.Status,
                DonViTienTe = _NhapKhoLyThuyet.DonViTienTe,
                CreatedDate = _NhapKhoLyThuyet.CreatedDate,
                ModifiedDate = _NhapKhoLyThuyet.ModifiedDate,
                CreatedBy = _NhapKhoLyThuyet.CreatedBy,
                ModifiedBy = _NhapKhoLyThuyet.ModifiedBy,
                Cancelled = _NhapKhoLyThuyet.Cancelled,
            };
        }

        public static implicit operator NhapKhoLyThuyet(NhapKhoLyThuyetCRUDViewModel vm)
        {
            return new NhapKhoLyThuyet
            {
                Id = vm.Id,
                NgayNhap = vm.NgayNhap,
                MoTa = vm.MoTa ?? string.Empty,
                Status = vm.Status,
                MaPhieu = vm.MaPhieu,
                MaSoLo = vm.MaSoLo,
                TenKhachHang = vm.TenKhachHang,
                DuKienNgayVe = vm.DuKienNgayVe,
                DonViTienTe = vm.DonViTienTe??"",
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
