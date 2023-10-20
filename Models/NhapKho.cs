using System;

namespace AMS.Models
{
    public class NhapKho : EntityBase
    {
        public int Id { get; set; }
        public DateTime NgayNhap { get; set; }
        public string MoTa { get; set; }
        public string DonViTienTe { get; set; }
    }
}