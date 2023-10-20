using AMS.Models.HistoryNhapXuatNguyenLieuViewModel;
using System;

namespace AMS.Models
{
    public class HistoryNhapXuatNguyenLieu
    {
        public int Id { get; set; }
        public int NguyenLieuId { get; set; }
        public double SoLuong { get; set; }
        public DateTime Ngay { get; set; }
        public bool Status { get; set; }
        
    }
}
