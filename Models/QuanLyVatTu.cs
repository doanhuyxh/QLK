using System;

namespace AMS.Models
{
    public class QuanLyVatTu : EntityBase
    {
        public int Id { get; set; }
        public string TenVatTu { get; set; }
        public int QuanLyLoaiVatTuId { get; set; }
        public string GhiChu { get; set; }
    }
}
