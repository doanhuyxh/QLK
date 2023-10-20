using System;

namespace AMS.Models
{
    public class QuanLyNhomNguoiDung : EntityBase
    {
        public int Id { get; set; }
        public string TenNhom { get; set; }
        public string MoTa { get; set; }
        public string Permistions { get; set; }
    }
}
