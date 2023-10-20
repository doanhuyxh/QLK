using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.NguyenLieuKiemKhoViewModel
{
    public class NguyenLieuKiemKhoViewModel : EntityBase
    {
        public int Id { get; set; }
        public int NguyenLieuId { get; set; }
        public int SoLuongThucTe { get; set; }
        public string ChatLuong { get; set; }
        public int KiemKhoId { get; set; }

        public static implicit operator NguyenLieuKiemKhoViewModel(NguyenLieuKiemKho _SanPham)
        {
            return new NguyenLieuKiemKhoViewModel
            {
                Id = _SanPham.Id,
                NguyenLieuId = _SanPham.NguyenLieuId,
                SoLuongThucTe = _SanPham.SoLuongThucTe,
                KiemKhoId = _SanPham.KiemKhoId,
                ChatLuong = _SanPham.ChatLuong,
                CreatedDate = _SanPham.CreatedDate,
                ModifiedDate = _SanPham.ModifiedDate,
                CreatedBy = _SanPham.CreatedBy,
                ModifiedBy = _SanPham.ModifiedBy,
                Cancelled = _SanPham.Cancelled,
            };
        }

        public static implicit operator NguyenLieuKiemKho(NguyenLieuKiemKhoViewModel vm)
        {
            return new NguyenLieuKiemKho
            {
                Id = vm.Id,
                NguyenLieuId = vm.NguyenLieuId,
                SoLuongThucTe = vm.SoLuongThucTe,
                KiemKhoId = vm.KiemKhoId,
                ChatLuong = vm.ChatLuong,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
