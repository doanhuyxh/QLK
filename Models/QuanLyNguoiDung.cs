using System;

namespace AMS.Models
{
    public class QuanLyNguoiDung : EntityBase
    {
        public int Id { get; set; }
        public string HoVaTen { get; set; }
        public string? AnhDaiDienPath { get; set; }
        public string SoDienThoai { get; set; }
        public string TaiKhoan { get; set; }
        public int QuanLyNhomNguoiDungId { get; set; }
        public DateTime NgayThem { get; set; }
        public string ChucVu { get; set; }
    }
}
