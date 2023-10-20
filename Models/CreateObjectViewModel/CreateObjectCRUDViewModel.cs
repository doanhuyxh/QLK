using System.ComponentModel.DataAnnotations;
using AMS.Models;
using AMS.Models.ObjectFieldViewModel;

namespace AMS.Models.CreateObjectViewModel
{
    public class CreateObjectCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public int CreateObjectId { get; set; }
        public string Description { get; set; }
        public bool? IsTemplate { get; set; }
        public List<ObjectFieldCRUDViewModel> ObjectFields { get; set; }
        public List<int>? itemsNeedDelete { get; set; }
        public static implicit operator CreateObjectCRUDViewModel(CreateObject _CreateObject)
        {
            return new CreateObjectCRUDViewModel
            {
                Id = _CreateObject.Id,
                Name = _CreateObject.Name,
                Label = _CreateObject.Label,
                IsTemplate = _CreateObject.IsTemplate,
                Description = _CreateObject.Description,
                CreateObjectId = _CreateObject.CreateObjectId,
                CreatedDate = _CreateObject.CreatedDate,
                ModifiedDate = _CreateObject.ModifiedDate,
                CreatedBy = _CreateObject.CreatedBy,
                ModifiedBy = _CreateObject.ModifiedBy,
                Cancelled = _CreateObject.Cancelled,
            };
        }

        public static implicit operator CreateObject(CreateObjectCRUDViewModel vm)
        {
            return new CreateObject
            {
                Id = vm.Id,
                Name = vm.Name,
                Label = vm.Label,
                IsTemplate = vm.IsTemplate,
                Description = vm.Description,
                CreateObjectId = vm.CreateObjectId,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
