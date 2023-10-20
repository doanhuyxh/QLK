using System;

namespace AMS.Models
{
    public class QuanLyMenu : EntityBase
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int CreateObjectId { get; set; }
        public string Icon { get; set; }
        public string Task { get; set; }
        public int? Ordering { get; set; }
        public string Name { get; set; }
        public string? Parameter { get; set; }
        public bool DevelopmentEnvironment { get; set; }
        public string Description { get; set; }
    }
}
