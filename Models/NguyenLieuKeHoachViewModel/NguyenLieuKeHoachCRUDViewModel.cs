using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.NguyenLieuKeHoachCRUDViewModel
{
    public class NguyenLieuKeHoachCRUDViewModel : EntityBase
    {
        public int Id { get; set; }
        public int NguyenLieuId { get; set; }
        public int KeHoachSanXuatId { get; set; }
        public DateTime NgayDuKien { get; set; }
        public int SanPhamId { get; set; }
        public string SoMetChi { get; set; }
        public string CachSuDung { get; set; }
        public string ListSizeMau { get; set; }
        public string DonVi { get; set; }
        public int SoLuong { get; set; }
        public string DinhMuc { get; set; }
        public string NhuCau { get; set; }
        public string SoLo { get; set; }
        public DateTime NgayVeVIT { get; set; }
        public string ThucNhan { get; set; }
        public string MieuTa { get; set; }

        public static implicit operator NguyenLieuKeHoachCRUDViewModel(NguyenLieuKeHoach _KeHoach)
        {
            return new NguyenLieuKeHoachCRUDViewModel
            {
                Id = _KeHoach.Id,
                NgayDuKien = _KeHoach.NgayDuKien,
                SanPhamId = _KeHoach.SanPhamId,
                SoMetChi = _KeHoach.SoMetChi,
                SoLo = _KeHoach.SoLo,
                SoLuong = _KeHoach.SoLuong,
                DinhMuc = _KeHoach.DinhMuc,
                DonVi = _KeHoach.DonVi,
                NgayVeVIT = _KeHoach.NgayVeVIT,
                NguyenLieuId = _KeHoach.NguyenLieuId,
                NhuCau = _KeHoach.NhuCau,
                ThucNhan = _KeHoach.ThucNhan,
                CachSuDung = _KeHoach.CachSuDung,
                ListSizeMau = _KeHoach.ListSizeMau,
                KeHoachSanXuatId = _KeHoach.KeHoachSanXuatId,
                MieuTa = _KeHoach.MieuTa,
                CreatedDate = _KeHoach.CreatedDate,
                ModifiedDate = _KeHoach.ModifiedDate,
                CreatedBy = _KeHoach.CreatedBy,
                ModifiedBy = _KeHoach.ModifiedBy,
                Cancelled = _KeHoach.Cancelled,

            };
        }

        public static implicit operator NguyenLieuKeHoach(NguyenLieuKeHoachCRUDViewModel vm)
        {
            return new NguyenLieuKeHoach
            {
                Id = vm.Id,
                NgayDuKien = vm.NgayDuKien,
                SanPhamId = vm.SanPhamId,
                SoMetChi = vm.SoMetChi,
                SoLo = vm.SoLo,
                SoLuong = vm.SoLuong,
                DinhMuc = vm.DinhMuc,
                DonVi = vm.DonVi,
                NgayVeVIT = vm.NgayVeVIT,
                NguyenLieuId = vm.NguyenLieuId,
                NhuCau = vm.NhuCau,
                ThucNhan = vm.ThucNhan,
                CachSuDung = vm.CachSuDung,
                ListSizeMau = vm.ListSizeMau,
                KeHoachSanXuatId = vm.KeHoachSanXuatId,
                MieuTa = vm.MieuTa,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
