using System;

namespace AMS.Models
{
    public class DanhMucLoaiThuoc : EntityBase
    {
        public int Id { get; set; }
        public string MaLoai { get; set; }
        public string TenLoai { get; set; }
        public int ParentId { get; set; }
        public string GhiChu { get; set; }
    }
}
