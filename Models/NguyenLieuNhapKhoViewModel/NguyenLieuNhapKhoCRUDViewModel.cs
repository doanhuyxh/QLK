using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.NguyenLieuNhapKhoCRUDViewModel
{
    public class NguyenLieuNhapKhoCRUDViewModel : EntityBase
    {
        public int Id { get; set; }
        public int NguyenLieuId { get; set; }
        public int NhapKhoId { get; set; }
        public string? ChatLuong { get; set; }
        public string Solo { get; set; }
        public string NhaCungCap { get; set; }
        public DateTime NgayNhap { get; set; }
        [Display(Name = "Số lượng nhập")]
        public int SoLuongNhap { get; set; }
        [Display(Name = "Số lượng lý thuyết")]
        public int SoLuongNhapTrenChungTu { get; set; }
        public int DonGia { get; set; }
        [Display(Name = "Chi tiết nguyên liệu")]
        public string? ChiTietCusTom { get; set; }
        [Display(Name = "Mã kho")]
        public string? MaKho { get; set; }
        public string? MaHaiQuan { get; set; }

        public static implicit operator NguyenLieuNhapKhoCRUDViewModel(NguyenLieuNhapKho _NhapKho)
        {
            return new NguyenLieuNhapKhoCRUDViewModel
            {
                Id = _NhapKho.Id,
                NgayNhap = _NhapKho.NgayNhap,
                ChatLuong = _NhapKho.ChatLuong,
                Solo = _NhapKho.Solo,
                DonGia = _NhapKho.DonGia,
                NguyenLieuId = _NhapKho.NguyenLieuId,
                NhaCungCap = _NhapKho.NhaCungCap,
                NhapKhoId = _NhapKho.NhapKhoId,
                SoLuongNhap = _NhapKho.SoLuongNhap,
                MaHaiQuan = _NhapKho.MaHaiQuan,
                SoLuongNhapTrenChungTu = _NhapKho.SoLuongNhapTrenChungTu,
                ChiTietCusTom = _NhapKho.ChiTietCusTom,
                MaKho = _NhapKho.MaKho,
                CreatedDate = _NhapKho.CreatedDate,
                ModifiedDate = _NhapKho.ModifiedDate,
                CreatedBy = _NhapKho.CreatedBy,
                ModifiedBy = _NhapKho.ModifiedBy,
                Cancelled = _NhapKho.Cancelled,
            };
        }

        public static implicit operator NguyenLieuNhapKho(NguyenLieuNhapKhoCRUDViewModel vm)
        {
            return new NguyenLieuNhapKho
            {
                Id = vm.Id,
                NgayNhap = vm.NgayNhap,
                ChatLuong = vm.ChatLuong ?? "",
                NguyenLieuId = vm.NguyenLieuId,
                NhaCungCap = vm.NhaCungCap,
                Solo = vm.Solo,
                NhapKhoId = vm.NhapKhoId,
                SoLuongNhap = vm.SoLuongNhap,
                MaHaiQuan = vm.MaHaiQuan ?? "",
                SoLuongNhapTrenChungTu = vm.SoLuongNhapTrenChungTu,
                ChiTietCusTom = vm.ChiTietCusTom ?? "",
                DonGia = vm.DonGia,
                MaKho = vm.MaKho ?? "",
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
