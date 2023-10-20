using System;

namespace AMS.Models
{
    public class TaiSan : EntityBase
    {
        public int Id { get; set; }
        public string MaTaiSan { get; set; }
        public string TenTaiSan { get; set; }
        public int SoLuong { get; set; }
        public string? HinhAnhPath { get; set; }
    }
}
