using System;

namespace AMS.Models
{
    public class QuanLyThanhPham : EntityBase
    {
        public int Id { get; set; }
        public string TenThanhPham { get; set; }
        public string GhiChu { get; set; }
        public string KhachHang { get; set; }
        public string Mau { get; set; }
        public string Size { get; set; }
        public string PO { get; set; }
        public int SoLuong { get; set; }
    }
}
