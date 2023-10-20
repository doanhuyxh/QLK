using System;

namespace AMS.Models
{
    public class QuanLyDonViSanXuat : EntityBase
    {
        public int Id { get; set; }
        public string TenDonVi { get; set; }
public string QuocGiaSanXuat { get; set; }
public string? AnhDonVi { get; set; }
public string GhiChu { get; set; }
    }
}
