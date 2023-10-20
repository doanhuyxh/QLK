using System;

namespace AMS.Models
{
    public class QuanLyKho : EntityBase
    {
        public int Id { get; set; }
        public string MaKho { get; set; }
        public string TenKho { get; set; }
        public int LoaiKhoId { get; set; }
        public int DanhMucLoaiThuocId { get; set; }
        public string GhiChu { get; set; }
    }
}
