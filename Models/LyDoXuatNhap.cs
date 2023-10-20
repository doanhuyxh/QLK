using System;

namespace AMS.Models
{
    public class LyDoXuatNhap : EntityBase
    {
        public int Id { get; set; }
        public string MaLyDo { get; set; }
public string TenLyDo { get; set; }
public string LoaiLyDo { get; set; }
public string GhiChu { get; set; }
    }
}
