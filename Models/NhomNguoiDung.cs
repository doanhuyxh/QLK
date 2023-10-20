using System;

namespace AMS.Models
{
    public class NhomNguoiDung : EntityBase
    {
        public int Id { get; set; }
        public string TenNhom { get; set; }
public int ParentId { get; set; }
public string QuyenHanNhom { get; set; }
public string MoTa { get; set; }
public string? AnhDaiDienPath { get; set; }
    }
}
