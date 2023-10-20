using System.ComponentModel.DataAnnotations;
using AMS.Models;
using AMS.Models.NguyenLieuNhapKhoLyThuyetViewModel;

namespace AMS.Models.NhapKhoLyThuyet2ViewModel
{
    public class NhapKhoLyThuyet2CRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }
        [Display(Name = "Dự kiến ngày về")]
        public DateTime DuKienNgayVe { get; set; }
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
        [Display(Name = "Số tờ khai")]

        public string SoToKhai { get; set; }

        public bool IsDue { get; set; }
        public List<NguyenLieuNhapKhoLyThuyet2CRUDViewModel> NguyenLieuNhapKhoLyThuyetList2 { get; set; }


        public static implicit operator NhapKhoLyThuyet2CRUDViewModel(NhapKhoLyThuyet2 _NhapKhoLyThuyet2)
        {
            return new NhapKhoLyThuyet2CRUDViewModel
            {
                Id = _NhapKhoLyThuyet2.Id,
                NgayNhap = _NhapKhoLyThuyet2.NgayNhap,
                MoTa = _NhapKhoLyThuyet2.MoTa,
                DuKienNgayVe = _NhapKhoLyThuyet2.DuKienNgayVe,
                MaPhieu = _NhapKhoLyThuyet2.MaPhieu,
                MaSoLo = _NhapKhoLyThuyet2.MaSoLo,
                TenKhachHang = _NhapKhoLyThuyet2.TenKhachHang,
                Status = _NhapKhoLyThuyet2.Status,
                DonViTienTe = _NhapKhoLyThuyet2.DonViTienTe,
                CreatedDate = _NhapKhoLyThuyet2.CreatedDate,
                ModifiedDate = _NhapKhoLyThuyet2.ModifiedDate,
                CreatedBy = _NhapKhoLyThuyet2.CreatedBy,
                ModifiedBy = _NhapKhoLyThuyet2.ModifiedBy,
                Cancelled = _NhapKhoLyThuyet2.Cancelled,
                SoToKhai = _NhapKhoLyThuyet2.SoToKhai,
            };
        }

        public static implicit operator NhapKhoLyThuyet2(NhapKhoLyThuyet2CRUDViewModel vm)
        {
            return new NhapKhoLyThuyet2
            {
                Id = vm.Id,
                NgayNhap = vm.NgayNhap,
                MoTa = vm.MoTa ?? string.Empty,
                Status = vm.Status,
                MaPhieu = vm.MaPhieu,
                MaSoLo = vm.MaSoLo,
                TenKhachHang = vm.TenKhachHang,
                DuKienNgayVe = vm.DuKienNgayVe,
                DonViTienTe = vm.DonViTienTe ?? "",
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
                SoToKhai = vm.SoToKhai,
            };
        }
    }
}
