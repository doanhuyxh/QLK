using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.ListNguyenLieuInCongThucViewModel
{
    public class ListNguyenLieuInCongThucViewModel : EntityBase
    {
        public int Id { get; set; }
        public string? MieuTa { get; set; }
        [Required(ErrorMessage = "Nguyên liệu bị trống")]
        public int NguyenLieuId { get; set; }
        public int SanPhamId { get; set; }
        public string? SoMetChi { get; set; }
        public string? CachSuDung { get; set; }
        public string? ListSizeMau { get; set; }
        [Required(ErrorMessage = "chưa chọn đơn vị")]
        public string? DonVi { get; set; }
        [Required(ErrorMessage = "Số lượng không hợp lệ")]
        public int SoLuong { get; set; }
        public string? DinhMuc { get; set; }
        public string? NhuCau { get; set; }
        public string? SoLo { get; set; }
        public DateTime NgayVeVIT { get; set; }
        public string? ThucNhan { get; set; }


        public string? TenNguyenLieuByCongThuc { get; set; }


        public static implicit operator ListNguyenLieuInCongThucViewModel(ListNguyenLieuInCongThuc _NguyenLieu)
        {
            return new ListNguyenLieuInCongThucViewModel
            {
                Id = _NguyenLieu.Id,
                MieuTa = _NguyenLieu.MieuTa,
                NguyenLieuId = _NguyenLieu.NguyenLieuId,
                SanPhamId = _NguyenLieu.SanPhamId,
                SoMetChi = _NguyenLieu.SoMetChi,
                CreatedBy = _NguyenLieu.CreatedBy,
                CreatedDate = _NguyenLieu.CreatedDate,
                ModifiedBy = _NguyenLieu.ModifiedBy,
                ModifiedDate = _NguyenLieu.ModifiedDate,
                Cancelled = _NguyenLieu.Cancelled,
                CachSuDung = _NguyenLieu.CachSuDung,
                ListSizeMau = _NguyenLieu.ListSizeMau,
                DonVi = _NguyenLieu.DonVi,
                SoLuong = _NguyenLieu.SoLuong,
                DinhMuc = _NguyenLieu.DinhMuc,
                NhuCau = _NguyenLieu.NhuCau,
                SoLo = _NguyenLieu.SoLo,
                NgayVeVIT = _NguyenLieu.NgayVeVIT,
                ThucNhan = _NguyenLieu.ThucNhan,
            };
        }

        public static implicit operator ListNguyenLieuInCongThuc(ListNguyenLieuInCongThucViewModel vm)
        {
            return new ListNguyenLieuInCongThuc
            {
                Id = vm.Id,
                MieuTa = vm.MieuTa,
                NguyenLieuId = vm.NguyenLieuId,
                SanPhamId = vm.SanPhamId,
                SoMetChi = vm.SoMetChi,
                CreatedBy = vm.CreatedBy,
                CreatedDate = vm.CreatedDate,
                ModifiedBy = vm.ModifiedBy,
                ModifiedDate = vm.ModifiedDate,
                Cancelled = vm.Cancelled,
                CachSuDung = vm.CachSuDung,
                ListSizeMau = vm.ListSizeMau,
                DonVi = vm.DonVi,
                SoLuong = vm.SoLuong,
                DinhMuc = vm.DinhMuc,
                NhuCau = vm.NhuCau,
                SoLo = vm.SoLo,
                NgayVeVIT = vm.NgayVeVIT,
                ThucNhan = vm.ThucNhan,
            };
        }
    }
}
