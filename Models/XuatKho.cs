using System;

namespace AMS.Models
{
    public class XuatKho : EntityBase
    {
        public int Id { get; set; }
        public string TenPhieuXuatKho { get; set; }
        public string DanhGia { get; set; }
        public string Status { get; set; }

    }
}
