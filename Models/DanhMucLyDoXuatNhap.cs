using System;

namespace AMS.Models
{
    public class DanhMucLyDoXuatNhap : EntityBase
    {
        public int Id { get; set; }
        public string MaLyDo { get; set; }
        public string TenLyDo { get; set; }
        public int LoaiLyDoTrongXuatNhapKhoId { get; set; }
        public string GhiChu { get; set; }
    }
}
