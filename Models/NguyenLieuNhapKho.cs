using System;

namespace AMS.Models
{
    public class NguyenLieuNhapKho : EntityBase
    {
        public int Id { get; set; }
        public int NguyenLieuId { get; set; }
        public int NhapKhoId { get; set; }
        public string? ChatLuong { get; set; }
        public string? NhaCungCap { get; set; }
        public string Solo { get; set; }
        public DateTime NgayNhap { get; set; }
        public int SoLuongNhap { get; set; }
        public int SoLuongNhapTrenChungTu { get; set; }
        public int DonGia { get; set; }
        public string? ChiTietCusTom { get; set; }
        public string? MaKho { get; set; }
        public string? MaHaiQuan { get; set; }
    }
}
