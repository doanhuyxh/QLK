using System;

namespace AMS.Models
{
    public class NhapThanhPham : EntityBase
    {
        public int Id { get; set; }
        public DateTime NgayNhap { get; set; }
        public string Ma { set; get; }
        public string DonViTienTe { set; get; }
    }
}
