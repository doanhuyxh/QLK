using System;

namespace AMS.Models
{
    public class LoaiKho : EntityBase
    {
        public int Id { get; set; }
        public string TenLoaiKho { get; set; }
        public string GhiChu { get; set; }
    }
}
