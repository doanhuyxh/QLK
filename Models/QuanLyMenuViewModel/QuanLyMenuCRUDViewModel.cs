using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.QuanLyMenuViewModel
{
    public class QuanLyMenuCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int CreateObjectId { get; set; }
        public int? Ordering { get; set; }
        public string Name { get; set; }
        public string? Label { get; set; }
        public string? Parameter { get; set; }
        public string? View { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Task { get; set; }
        public int? TotalItemOfParent { get; set; }
        [Display(Name = "Dành cho môi trường dev")]
        public bool DevelopmentEnvironment { get; set; }
        public List<QuanLyMenuCRUDViewModel>? Children { get; set; }

        public static implicit operator QuanLyMenuCRUDViewModel(QuanLyMenu _QuanLyMenu)
        {
            return new QuanLyMenuCRUDViewModel
            {
                Id = _QuanLyMenu.Id,
                ParentId = _QuanLyMenu.ParentId,
                Parameter = _QuanLyMenu.Parameter,
                DevelopmentEnvironment = _QuanLyMenu.DevelopmentEnvironment,
                Icon = _QuanLyMenu.Icon,
                CreateObjectId = _QuanLyMenu.CreateObjectId,
                Ordering = _QuanLyMenu.Ordering,
                Name = _QuanLyMenu.Name,
                Task = _QuanLyMenu.Task,
                Description = _QuanLyMenu.Description,
                CreatedDate = _QuanLyMenu.CreatedDate,
                ModifiedDate = _QuanLyMenu.ModifiedDate,
                CreatedBy = _QuanLyMenu.CreatedBy,
                ModifiedBy = _QuanLyMenu.ModifiedBy,
                Cancelled = _QuanLyMenu.Cancelled,
            };
        }

        public static implicit operator QuanLyMenu(QuanLyMenuCRUDViewModel vm)
        {
            return new QuanLyMenu
            {
                Id = vm.Id,
                ParentId = vm.ParentId,
                Icon = vm.Icon,
                CreateObjectId = vm.CreateObjectId,
                DevelopmentEnvironment = vm.DevelopmentEnvironment,
                Ordering = vm.Ordering,
                Name = vm.Name,
                Task = vm.Task,
                Parameter = vm.Parameter,
                Description = vm.Description,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
