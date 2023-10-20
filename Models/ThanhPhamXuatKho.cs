using System;

namespace AMS.Models
{
    public class ThanhPhamXuatKho : EntityBase
    {
        public int Id { get; set; }
        public int ThanhPhamId { get; set; }
        public int XuatKhoThanhPhamId { get; set; }
        public int SoLuong { get; set; }
        public int DonGia { get; set; }
        public string PO { get; set; }
        public string Size { get; set; }
        public string Mau { get; set; }
        public DateTime NgayXuat { get; set; }
        public string KhachHang { get; set; }
        public string MaHang { get; set; }
    }
}
