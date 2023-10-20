using System;

namespace AMS.Models
{
    public class DisplayFieldOfTable : EntityBase
    {
        public int Id { get; set; }
        public int ObjectFiledId { get; set; }
        public string TenTruong { get; set; }
        public string MoTa { get; set; }
    }
}
