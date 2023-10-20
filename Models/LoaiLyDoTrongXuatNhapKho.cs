using System;

namespace AMS.Models
{
    public class LoaiLyDoTrongXuatNhapKho : EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MoTa { get; set; }
        public string NhapKho { get; set; }
        public string XuatKho { get; set; }
    }
}
