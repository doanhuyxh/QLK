using System;

namespace AMS.Models
{
    public class KhachHang : EntityBase
    {
        public int Id { get; set; }
        public string HoVaTen { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string GhiChu { get; set; }
        public int NhomKhachHangId { get; set; }
    }
}
