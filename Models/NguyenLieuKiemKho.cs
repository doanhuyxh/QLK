using System;

namespace AMS.Models
{
    public class NguyenLieuKiemKho : EntityBase
    {
        public int Id { get; set; }
        public int NguyenLieuId { get; set; }
        public int SoLuongThucTe { get; set; }
        public string ChatLuong { get; set; }
        public int KiemKhoId { get; set; }        
    }
}
