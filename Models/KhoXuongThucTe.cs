using System;

namespace AMS.Models
{
    public class KhoXuongThucTe : EntityBase
    {
        public int Id { get; set; }
        public string TenKho { get; set; }
        public int ParentId { get; set; }
        public string GhiChu { get; set; }
    }
}
