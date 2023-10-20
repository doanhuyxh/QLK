using System;

namespace AMS.Models
{
    public class TenTaiSan : EntityBase
    {
        public int Id { get; set; }
        public string LoaiTenTaiSan { get; set; }
        public string MaTaiSan { get; set; }
        public DateTime NgayNhap { get; set; }
    }
}
