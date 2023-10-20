using System;

namespace AMS.Models
{
    public class CustomFieldTotal
    {
        public int ID { get; set; }
        public int NguyenLieuID { get; set; }
        public string ListCustom { get; set; }
        public int QuantityProduct { get; set; }
        public bool Cancel { get; set; }
    }
}
