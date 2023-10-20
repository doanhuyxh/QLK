using System;

namespace AMS.Models
{
    public class SubViewField : EntityBase
    {
        public int Id { get; set; }
        public int? OjbectFieldId { get; set; }
        public string TenTruong { get; set; }
        public string KieuDuLieu { get; set; }
        public string Label { get; set; }
        public int? DoLon { get; set; }
        public bool HienThiTrongBang { get; set; }
        public bool TreeView { get; set; }
        public bool BatBuocNhap { get; set; }
    }
}
