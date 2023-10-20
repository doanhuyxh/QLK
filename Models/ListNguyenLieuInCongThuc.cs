using System;

namespace AMS.Models
{
    public class ListNguyenLieuInCongThuc : EntityBase
    {
        public int Id { get; set; }
        public string? MieuTa { get; set; }
        public int NguyenLieuId { get; set; }
        public int SanPhamId { get; set; }
        public string? SoMetChi { get; set; }
        public string? CachSuDung { get; set; }
        public string? ListSizeMau { get; set; }
        public string? DonVi { get; set; }
        public int SoLuong { get; set; }
        public string? DinhMuc { get; set; }
        public string? NhuCau { get; set; }
        public string? SoLo { get; set; }
        public DateTime NgayVeVIT { get; set; }
        public string? ThucNhan { get; set; }

    }
}
