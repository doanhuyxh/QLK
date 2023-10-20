using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.NguyenLieuXuatKhoCRUDViewModel
{
    public class NguyenLieuXuatKhoCRUDViewModel : EntityBase
    {
        public int Id { get; set; }
        public int NguyenLieuId { get; set; }
        public int XuatKhoId { get; set; }
        [Display(Name ="Chất lượng")]
        public string ChatLuong { get; set; }
        public DateTime NgayXuat { get; set; }
        [Display(Name = "Số lượng xuất")]
        public int SoLuongXuat { get; set; }
        [Display(Name = "Đơn vị")]
        public string DonVi { get; set; }
        [Display(Name = "Chi tiết nguyên liệu")]
        public string? ChiTietCusTom { get; set; }

        public static implicit operator NguyenLieuXuatKhoCRUDViewModel(NguyenLieuXuatKho _XuatKho)
        {
            return new NguyenLieuXuatKhoCRUDViewModel
            {
                Id = _XuatKho.Id,
                NgayXuat = _XuatKho.NgayXuat,
                ChatLuong = _XuatKho.ChatLuong,
                NguyenLieuId = _XuatKho.NguyenLieuId,
                XuatKhoId = _XuatKho.XuatKhoId,
                SoLuongXuat = _XuatKho.SoLuongXuat,
                CreatedDate = _XuatKho.CreatedDate,
                ModifiedDate = _XuatKho.ModifiedDate,
                CreatedBy = _XuatKho.CreatedBy,
                ModifiedBy = _XuatKho.ModifiedBy,
                Cancelled = _XuatKho.Cancelled,
                DonVi = _XuatKho.DonVi,
                ChiTietCusTom = _XuatKho.ChiTietCusTom
            };
        }

        public static implicit operator NguyenLieuXuatKho(NguyenLieuXuatKhoCRUDViewModel vm)
        {
            return new NguyenLieuXuatKho
            {
                Id = vm.Id,
                NgayXuat = vm.NgayXuat,
                ChatLuong = vm.ChatLuong,
                NguyenLieuId = vm.NguyenLieuId,
                XuatKhoId = vm.XuatKhoId,
                SoLuongXuat = vm.SoLuongXuat,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
                DonVi = vm.DonVi,
                ChiTietCusTom = vm.ChiTietCusTom??"",
            };
        }
    }
}
