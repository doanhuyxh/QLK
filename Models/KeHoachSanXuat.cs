using System;

namespace AMS.Models
{
    public class KeHoachSanXuat : EntityBase
    {
        public int Id { get; set; }
        public string TenThanhPham { get; set; }
        public DateTime NgayDuKienHoan { get; set; }
        public int? SoLuongThanhPham { get; set; }
    }
}
