using System;

namespace AMS.Models
{
    public class PhieuXuatThanhPham : EntityBase
    {
        public int Id { get; set; }
        public string MaPhieuXuat { get; set; }
        public DateTime NgayDuKienXuat { get; set; }
    }
}
