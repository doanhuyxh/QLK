using System;

namespace AMS.Models
{
    public class KiemThu : EntityBase
    {
        public int Id { get; set; }
        public string MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string DonViTinh { get; set; }
        public int LoaiThuocId { get; set; }
        public int NuocSanXuatId { get; set; }
        public string MaVach { get; set; }
        public string QuyCach { get; set; }
        public string HoatChat { get; set; }
    }
}
