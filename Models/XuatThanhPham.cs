using System;

namespace AMS.Models
{
    public class XuatThanhPham : EntityBase
    {
        public int Id { get; set; }
        public DateTime NgayXuat { get; set; }
        public string DonViTienTe { get; set; }
    }
}
