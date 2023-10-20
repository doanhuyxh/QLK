using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.HistoryNhapXuatNguyenLieuViewModel
{
    public class HistoryNhapXuatNguyenLieuCRUDViewModel : EntityBase
    {
        public int Id { get; set; }
        public int NguyenLieuId { get; set; }
        public double SoLuong { get; set; }
        public DateTime Ngay { get; set; }
        public bool Status { get; set; }
        public string NameNguyenLieu { get; set; }
        public double TotalNhap { get; set; }
        public double TotalXuat { get; set; }
        public DateTime? NgayNhap { get; set; }
        public DateTime? NgayXuat { get; set; }
        
        public string DonVi { get; set; }

        public double SoLuongDangCo { get; set; }






        public static implicit operator HistoryNhapXuatNguyenLieuCRUDViewModel(HistoryNhapXuatNguyenLieu _History)
        {
            return new HistoryNhapXuatNguyenLieuCRUDViewModel
            {
                Id = _History.Id,
                NguyenLieuId = _History.NguyenLieuId,
                SoLuong = _History.SoLuong,
                Ngay = _History.Ngay,
                Status = _History.Status
            };
        }

        public static implicit operator HistoryNhapXuatNguyenLieu(HistoryNhapXuatNguyenLieuCRUDViewModel vm)
        {
            return new HistoryNhapXuatNguyenLieu
            {

                Id = vm.Id,
                NguyenLieuId = vm.NguyenLieuId,
                SoLuong = vm.SoLuong,
                Ngay = vm.Ngay,
                Status = vm.Status
            };
        }
    }
}
