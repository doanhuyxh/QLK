using System;

namespace AMS.Models
{
    public class QuanLyLoaiVatTu : EntityBase
    {
        public int Id { get; set; }
        public string TenLoaiVatTu { get; set; }
public int ParentId { get; set; }
public string GhiChu { get; set; }
    }
}
