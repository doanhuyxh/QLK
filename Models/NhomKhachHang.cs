using System;

namespace AMS.Models
{
    public class NhomKhachHang : EntityBase
    {
        public int Id { get; set; }
        public string TenNhomKhachHang { get; set; }
        public string GhiChu { get; set; }
    }
}
