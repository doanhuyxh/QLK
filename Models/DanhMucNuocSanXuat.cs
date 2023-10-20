using System;

namespace AMS.Models
{
    public class DanhMucNuocSanXuat : EntityBase
    {
        public int Id { get; set; }
        public string MaNuoc { get; set; }
public string TenNuoc { get; set; }
public string GhiChu { get; set; }
public string? HinhAnhPath { get; set; }
    }
}
