using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.DisplayFieldOfTableViewModel
{
    public class DisplayFieldOfTableCRUDViewModel : EntityBase
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
        public static implicit operator DisplayFieldOfTableCRUDViewModel(DisplayFieldOfTable _DisplayFieldOfTable)
        {
            return new DisplayFieldOfTableCRUDViewModel
            {
                Id = _DisplayFieldOfTable.Id,
                ObjectFiledId = _DisplayFieldOfTable.ObjectFiledId,
                TenTruong = _DisplayFieldOfTable.TenTruong,
                MoTa = _DisplayFieldOfTable.MoTa,
                CreatedDate = _DisplayFieldOfTable.CreatedDate,
                ModifiedDate = _DisplayFieldOfTable.ModifiedDate,
                CreatedBy = _DisplayFieldOfTable.CreatedBy,
                ModifiedBy = _DisplayFieldOfTable.ModifiedBy,
                Cancelled = _DisplayFieldOfTable.Cancelled,
            };
        }

        public static implicit operator DisplayFieldOfTable(DisplayFieldOfTableCRUDViewModel vm)
        {
            return new DisplayFieldOfTable
            {
                Id = vm.Id,
                ObjectFiledId = vm.ObjectFiledId,
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
