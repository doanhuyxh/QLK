using System;

namespace AMS.Models
{
    public class NhapKhoLyThuyet2 : EntityBase
    {
        public int Id { get; set; }
        public string MaPhieu { get; set; }
        public string MaSoLo { get; set; }
        public string TenKhachHang { get; set; }
        public DateTime NgayNhap { get; set; }
        public DateTime DuKienNgayVe { get; set; }
        public string MoTa { get; set; }
        public bool Status { get; set; }
        public string DonViTienTe { get; set; }
        public string SoToKhai { get; set; }
    }
}
