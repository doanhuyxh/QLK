using System;

namespace AMS.Models
{
    public class DonThuoc : EntityBase
    {
        public int Id { get; set; }
        public string TenDonThuoc { get; set; }
        public string HoVaTenQuanNhan { get; set; }
        public bool GioiTinh { set;get; }
        public string NamSinh { get; set; }
        public string CapBac { get;set; }
        public string DonViCongTac { get; set; }
        public string MaBaoHiemYTe { get; set; }
        public string ChuanDoan { set;get; }
    }
}
