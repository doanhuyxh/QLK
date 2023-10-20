using System;

namespace AMS.Models
{
    public class DanhMucThuoc : EntityBase
    {
        public int Id { get; set; }
        public string MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public int DonViThuocId { get; set; }
        public int DanhMucLoaiThuocId { get; set; }
        public string GhiChu { get; set; }
        public string HamLuong { get; set; }
        public string NongDo { get; set; }
        public string HinhAnhPath { get; set; }
        public int DanhMucNuocSanXuatId { get; set; }
        public int DonViSanXuatId { get; set; }
        public int HanDung { set; get; }
    }
}
