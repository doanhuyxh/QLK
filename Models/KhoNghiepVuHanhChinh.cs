using System;

namespace AMS.Models
{
    public class KhoNghiepVuHanhChinh : EntityBase
    {
        public int Id { get; set; }
        public string MaKho { get; set; }
        public string TenKho { get; set; }
        public string GhiChu { get; set; }
        public int LoaiThuocId { get; set; }
    }
}
