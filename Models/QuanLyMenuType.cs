using System;

namespace AMS.Models
{
    public class QuanLyMenuType : EntityBase
    {
        public int Id { get; set; }
        public string TenMenuType { get; set; }
        public string MoTa { get; set; }
        public bool LaMenuFrontend { get; set; }
    }
}
