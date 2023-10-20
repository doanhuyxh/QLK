using System.ComponentModel.DataAnnotations;

namespace AMS.Models.NhapKhoViewModel
{
    public class ReportNhapKho
    {
        public string MaNguyenLieu { set; get; }
        public string TenNguyenLieu { set; get; }
        public int SoLuongNhap { set; get; }
        public int DonGia { set; get; }

    }
}
