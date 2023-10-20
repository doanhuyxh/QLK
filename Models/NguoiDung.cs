using System;

namespace AMS.Models
{
    public class NguoiDung : EntityBase
    {
        public int Id { get; set; }
        public string HoVaTen { get; set; }
public string Username { get; set; }
public string Password { get; set; }
public string DiaChi { get; set; }
public int NhomNguoiDungId { get; set; }
public bool IsSupperAdmin { get; set; }
    }
}
