using System;

namespace AMS.Models
{
    public class QuanLyKhoXuongThucTe : EntityBase
    {
        public int Id { get; set; }
        public string TenKho { get; set; }
public int QuanLyKhoXuongId { get; set; }
public string GhiChu { get; set; }
    }
}
