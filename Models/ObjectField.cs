using System;

namespace AMS.Models
{
    public class ObjectField : EntityBase
    {
        public int Id { get; set; }
        public int CreateObjectId { get; set; }
        public string TenTruong { get; set; }
        public string KieuDuLieu { get; set; }
        public string Label { get; set; }
        public string? TenBang { get; set; }
        public string? LabelTenBang { get; set; }
        public int? DoLon { get; set; }
        public bool HienThiTrongBang { get; set; }
        public bool TreeView { get; set; }
        public bool BatBuocNhap { get; set; }
        public int? RowIndex { get; set; }
        public int? ColumnIndex { get; set; }
    }
}
