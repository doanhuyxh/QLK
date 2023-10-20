using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.RelationshipTableViewModel
{
    public class RelationshipTableCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }
        [Display(Name = "Tên bảng")]
        public string TenBang { get; set; }
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }
        public static implicit operator RelationshipTableCRUDViewModel(RelationshipTable _RelationshipTable)
        {
            return new RelationshipTableCRUDViewModel
            {
                Id = _RelationshipTable.Id,
                TenBang = _RelationshipTable.TenBang,
                MoTa = _RelationshipTable.MoTa,
                CreatedDate = _RelationshipTable.CreatedDate,
                ModifiedDate = _RelationshipTable.ModifiedDate,
                CreatedBy = _RelationshipTable.CreatedBy,
                ModifiedBy = _RelationshipTable.ModifiedBy,
                Cancelled = _RelationshipTable.Cancelled,
            };
        }

        public static implicit operator RelationshipTable(RelationshipTableCRUDViewModel vm)
        {
            return new RelationshipTable
            {
                Id = vm.Id,
                TenBang = vm.TenBang,
                MoTa = vm.MoTa,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
