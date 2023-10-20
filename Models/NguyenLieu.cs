using System;

namespace AMS.Models
{
    public class NguyenLieu : EntityBase
    {
        public int Id { get; set; }
        public string MaNguyenLieu { get; set; }
        public string TenNguyenLieu { get; set; }
        public int SoLuong { get; set; }
        public int SoLuongLyThuyet { get; set; }
        public string DonViTinh { get; set; }
        public int KhoId { get; set; }
    }
}
