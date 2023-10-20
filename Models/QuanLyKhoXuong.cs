using System;

namespace AMS.Models
{
    public class QuanLyKhoXuong : EntityBase
    {
        public int Id { get; set; }
        public string TenKho { get; set; }
        public string GhiChu { get; set; }
        public int ParentId { get; set; }
    }
}
