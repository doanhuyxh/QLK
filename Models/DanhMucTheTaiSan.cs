using System;

namespace AMS.Models
{
    public class DanhMucTheTaiSan : EntityBase
    {
        public int Id { get; set; }
        public string SoThe { get; set; }
        public string KyHieu { get; set; }
        public string KieuMay { get; set; }
        public string NamSanXuat { get; set; }
        public DateTime ThoiGianMua { get; set; }
        public DateTime ThoiGianDuaVaoSuDung { get; set; }
        public string BoPhanSuDung { get; set; }
        public int DanhMucNuocSanXuatId { get; set; }
        public int TenTaiSanId { get; internal set; }
    }
}
