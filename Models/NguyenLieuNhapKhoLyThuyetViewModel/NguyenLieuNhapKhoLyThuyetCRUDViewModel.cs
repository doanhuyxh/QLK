using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.NguyenLieuNhapKhoLyThuyetViewModel
{
    public class NguyenLieuNhapKhoLyThuyetCRUDViewModel : EntityBase
    {
        public int Id { get; set; }
        public int NhapKhoLyThuyetId { get; set; }
        public string MaHaiQuan { get; set; }
        public string MaNhaCungCap { get; set; }
        public string NhaCungCap { get; set; }
        public int NguyenLieuId { get; set; }
        public int SoLuongMua { get; set; }
        public int DonGia { get; set; }
        public string? GhiChu { get; set; }
        public string DonViTinh { get; set; }
        public string ThanhPhan { get; set; }

        public DateTime NgayNhap { get; set; }

        public string? DateDisplay { get; set; }

        public static implicit operator NguyenLieuNhapKhoLyThuyetCRUDViewModel(NguyenLieuNhapKhoLyThuyet _NhapKho)
        {
            return new NguyenLieuNhapKhoLyThuyetCRUDViewModel
            {
                Id = _NhapKho.Id,
                NhapKhoLyThuyetId = _NhapKho.NhapKhoLyThuyetId,
                MaHaiQuan = _NhapKho.MaHaiQuan,
                MaNhaCungCap = _NhapKho.MaNhaCungCap,
                NguyenLieuId = _NhapKho.NguyenLieuId,
                NhaCungCap = _NhapKho.NhaCungCap,
                SoLuongMua = _NhapKho.SoLuongMua,
                DonGia = _NhapKho.DonGia,
                GhiChu = _NhapKho.GhiChu,
                NgayNhap = _NhapKho.NgayNhap,
                CreatedDate = _NhapKho.CreatedDate,
                ModifiedDate = _NhapKho.ModifiedDate,
                CreatedBy = _NhapKho.CreatedBy,
                ModifiedBy = _NhapKho.ModifiedBy,
                Cancelled = _NhapKho.Cancelled,
                DonViTinh = _NhapKho.DonViTinh,
                ThanhPhan = _NhapKho.ThanhPhan,
            };
        }

        public static implicit operator NguyenLieuNhapKhoLyThuyet(NguyenLieuNhapKhoLyThuyetCRUDViewModel vm)
        {
            return new NguyenLieuNhapKhoLyThuyet
            {
                Id = vm.Id,
                NhapKhoLyThuyetId = vm.NhapKhoLyThuyetId,
                MaHaiQuan = vm.MaHaiQuan,
                MaNhaCungCap = vm.MaNhaCungCap,
                NguyenLieuId = vm.NguyenLieuId,
                NhaCungCap = vm.NhaCungCap,
                SoLuongMua = vm.SoLuongMua,
                DonGia = vm.DonGia,
                GhiChu = vm.GhiChu ?? "",
                NgayNhap = vm.NgayNhap,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
                DonViTinh = vm.DonViTinh,
                ThanhPhan = vm.ThanhPhan,
            };
        }
    }
}
