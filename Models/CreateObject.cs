using System;

namespace AMS.Models
{
    public class CreateObject : EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public bool? IsTemplate { get; set; }
        public int CreateObjectId { get; set; }
        public string Description { get; set; }
    }
}
