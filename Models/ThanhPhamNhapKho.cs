using System;

namespace AMS.Models
{
    public class ThanhPhamNhapKho : EntityBase
    {
        public int Id { get; set; }
        public int ThanhPhamId { get; set; }
        public int NhapKhoThanhPhamId { get; set; }
        public int SoLuong { get; set; }
        public int DonGia { get; set; }
        public string PO { get; set; }
        public string Size { get; set; }
        public string Mau { get; set; }
        public string KhachHang { get; set; }
        public string MaHang { get; set; }
        public DateTime NgayNhap { get; set; }
    }
}
