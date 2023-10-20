using System;

namespace AMS.Models
{
    public class ThuocTheoDon : EntityBase
    {
        public int Id { get; set; }
        public int DanhMucThuocId { get; set; }
        public int SoLuong { get; set; }
        public int DonThuocId { get; set; }
        public string CachDung { get; set; }
    }
}
