using System;

namespace AMS.Models
{
    public class KiemKho : EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime NgayKiem { get; set; }
        public int? KhoId { get; set; }
        public int NhanVienId { get; set; }
    }
}
