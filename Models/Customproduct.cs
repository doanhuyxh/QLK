using System;

namespace AMS.Models
{
    public class Customproduct : EntityBase
    {
        public int Id { get; set; }
        public string Inforproduct { get; set; }
        public int Parentid { get; set; }
    }
}
    