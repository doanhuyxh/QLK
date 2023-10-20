using System;

namespace AMS.Models
{
    public class NhapKhoVatTu : EntityBase
    {
        public int Id { get; set; }
        public string? MaChungTu { get; set; }
        public DateTime NgayChungTu { get; set; }
        public int? QuanLyKhoId { get; set; }
        public string? NguoiNhap { get; set; }
        public int? DanhMucDonViId { get; set; }
        public string? TrangThaiChungTu { get; set; }
        public string? NguoiNhan { get; set; }
        public string? GhiChu { get; set; }
        public string? SoPhieu { get; set; }
    }
}
