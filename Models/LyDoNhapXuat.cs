using System;

namespace AMS.Models
{
    public class LyDoNhapXuat : EntityBase
    {
        public int Id { get; set; }
        public string MaLyDo { get; set; }
public string TenLyDo { get; set; }
public string GhiChu { get; set; }
public int LoaiLyDoId { get; set; }
    }
}
