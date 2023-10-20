using System;

namespace AMS.Models
{
    public class SubDepartment : EntityBase
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
