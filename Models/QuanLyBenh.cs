using System;

namespace AMS.Models
{
    public class QuanLyBenh : EntityBase
    {
        public int Id { get; set; }
        public string TenBenh { get; set; }
public int QuanLyLoaiBenhId { get; set; }
public string TrieuChung { get; set; }
    }
}
