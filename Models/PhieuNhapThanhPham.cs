using System;

namespace AMS.Models
{
    public class PhieuNhapThanhPham : EntityBase
    {
        public int Id { get; set; }
        public string MaNhap { get; set; }
        public DateTime NgayDuKienNhap { get; set; }
        public string DonViTienTe { get; set; }
    }
}
