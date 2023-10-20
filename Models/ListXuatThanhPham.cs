using System;

namespace AMS.Models
{
    public class ListXuatThanhPham : EntityBase
    {
        public int ID { get; set; }
        public int IDPhieu { get; set; }
        public int IDThanhPham { get; set; }
        public string? PO { get; set; }
        public string? Mau { get; set; }
        public string? Size { get; set; }
        public int SoLuong { get; set; }
        public int DonGia { get; set; }
        public string KhachHang { get; set; }
        public string MaHang { get; set; }
        public DateTime NgayNhap { get; set; }


    }
}
