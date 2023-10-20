using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.DisplaySubViewFieldOfTableViewModel
{
    public class DisplaySubViewFieldOfTableCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }
        public int ObjectFiledId { get; set; }
        [Display(Name = "Tên trường")]
        public string TenTruong { get; set; }
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }
        public bool Selected { get; set; }
        public static implicit operator DisplaySubViewFieldOfTableCRUDViewModel(DisplaySubViewFieldOfTable _DisplaySubViewFieldOfTable)
        {
            return new DisplaySubViewFieldOfTableCRUDViewModel
            {
                Id = _DisplaySubViewFieldOfTable.Id,
                TenTruong = _DisplaySubViewFieldOfTable.TenTruong,
                MoTa = _DisplaySubViewFieldOfTable.MoTa,
                CreatedDate = _DisplaySubViewFieldOfTable.CreatedDate,
                ModifiedDate = _DisplaySubViewFieldOfTable.ModifiedDate,
                CreatedBy = _DisplaySubViewFieldOfTable.CreatedBy,
                ModifiedBy = _DisplaySubViewFieldOfTable.ModifiedBy,
                Cancelled = _DisplaySubViewFieldOfTable.Cancelled,
            };
        }

        public static implicit operator DisplaySubViewFieldOfTable(DisplaySubViewFieldOfTableCRUDViewModel vm)
        {
            return new DisplaySubViewFieldOfTable
            {
                Id = vm.Id,
                TenTruong = vm.TenTruong,
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
