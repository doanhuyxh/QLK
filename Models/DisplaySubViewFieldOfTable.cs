using System;

namespace AMS.Models
{
    public class DisplaySubViewFieldOfTable : EntityBase
    {
        public int Id { get; set; }
        public int ObjectSubViewFiledId { get; set; }
        public string TenTruong { get; set; }
        public string MoTa { get; set; }
    }
}
