using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.KhoXuongThucTeViewModel
{
    public class KhoXuongThucTeCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Tên kho")]
        public string TenKho { get; set; }


        [Display(Name = "Thuộc kho cha")]
        public int ParentId { get; set; }

        [Display(Name = "Thuộc kho cha")]
        public string? ParentName { get; set; }


        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }

        public static implicit operator KhoXuongThucTeCRUDViewModel(KhoXuongThucTe _KhoXuongThucTe)
        {
            return new KhoXuongThucTeCRUDViewModel
            {
                Id = _KhoXuongThucTe.Id,
                TenKho = _KhoXuongThucTe.TenKho,
                ParentId = _KhoXuongThucTe.ParentId,
                GhiChu = _KhoXuongThucTe.GhiChu,
                CreatedDate = _KhoXuongThucTe.CreatedDate,
                ModifiedDate = _KhoXuongThucTe.ModifiedDate,
                CreatedBy = _KhoXuongThucTe.CreatedBy,
                ModifiedBy = _KhoXuongThucTe.ModifiedBy,
                Cancelled = _KhoXuongThucTe.Cancelled,
            };
        }

        public static implicit operator KhoXuongThucTe(KhoXuongThucTeCRUDViewModel vm)
        {
            return new KhoXuongThucTe
            {
                Id = vm.Id,
                TenKho = vm.TenKho,
                ParentId = vm.ParentId,
                GhiChu = vm.GhiChu,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
