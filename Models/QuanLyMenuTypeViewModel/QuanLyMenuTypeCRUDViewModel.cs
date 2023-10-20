using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.QuanLyMenuTypeViewModel
{
    public class QuanLyMenuTypeCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Tên menu type")]
        public string TenMenuType { get; set; }


        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }


        [Display(Name = "Là menu frontend")]
        public bool LaMenuFrontend { get; set; }

        public static implicit operator QuanLyMenuTypeCRUDViewModel(QuanLyMenuType _QuanLyMenuType)
        {
            return new QuanLyMenuTypeCRUDViewModel
            {
                Id = _QuanLyMenuType.Id,
                TenMenuType = _QuanLyMenuType.TenMenuType,
                MoTa = _QuanLyMenuType.MoTa,
                LaMenuFrontend = _QuanLyMenuType.LaMenuFrontend,
                CreatedDate = _QuanLyMenuType.CreatedDate,
                ModifiedDate = _QuanLyMenuType.ModifiedDate,
                CreatedBy = _QuanLyMenuType.CreatedBy,
                ModifiedBy = _QuanLyMenuType.ModifiedBy,
                Cancelled = _QuanLyMenuType.Cancelled,
            };
        }

        public static implicit operator QuanLyMenuType(QuanLyMenuTypeCRUDViewModel vm)
        {
            return new QuanLyMenuType
            {
                Id = vm.Id,
                TenMenuType = vm.TenMenuType,
                MoTa = vm.MoTa,
                LaMenuFrontend = vm.LaMenuFrontend,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
