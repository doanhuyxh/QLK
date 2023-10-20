using System;

namespace AMS.Models
{
    public class NguyenLieuXuatKho : EntityBase
    {
        public int Id { get; set; }
        public int NguyenLieuId { get; set; }
        public int XuatKhoId { get; set; }
        public string ChatLuong { get; set; }
        public DateTime NgayXuat { get; set; }
        public int SoLuongXuat { get; set; }
        public string DonVi { get; set; }
        public string ChiTietCusTom { get; set; }

    }
}
