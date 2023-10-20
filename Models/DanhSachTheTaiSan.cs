using System;

namespace AMS.Models
{
    public class DanhSachTheTaiSan : EntityBase
    {
        public int Id { get; set; }
        public int TenTaiSanId { get; set; }
        public string SoThe { get; set; }
        public string KyHieu { get; set; }
        public string KieuMay { get; set; }
        public string NamSanXuat { get; set; }
        public DateTime ThoiGianMua { get; set; }
        public DateTime ThoiGianDuaVaoSuDung { get; set; }
        public string GiaGoc { get; set; }
        public string BoPhanSuDung { get; set; }
        public int NuocSanXuatId { get; set; }
    }
}
