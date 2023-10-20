using System;

namespace AMS.Models
{
    public class NuocSanXuat : EntityBase
    {
        public int Id { get; set; }
        public string MaNuoc { get; set; }
        public string TenNuoc { get; set; }
        public string GhiChu { get; set; }
    }
}
