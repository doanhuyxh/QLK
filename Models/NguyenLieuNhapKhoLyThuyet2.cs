using System;

namespace AMS.Models
{
    public class NguyenLieuNhapKhoLyThuyet2 : EntityBase
    {
        public string MaNhaCungCap { get; set; }
        public string MaHaiQuan { get; set; }
        public string NhaCungCap { get; set; }
        public int NguyenLieuId { get; set; }
        public string ThanhPhan { get; set; }
        public string DonViTinh { get; set; }
        public int SoLuongMua { get; set; }
        public int DonGia { get; set; }
        public DateTime NgayNhap { get; set; }
        public string GhiChu { get; set; }
        public int Id { get; set; }
        public int NhapKhoLyThuyetId { get; set; }
    }
}
