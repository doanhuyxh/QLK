using System.ComponentModel.DataAnnotations;
using AMS.Models;
//[INSERT_IMPORT_FIELD_MODEL_VIEW]
namespace AMS.Models.ViewItemTemplateViewModel
{
    public class ViewItemTemplateCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }
        //[INSERT_FIELD_MODEL_VIEW]
        public static implicit operator ViewItemTemplateCRUDViewModel(ViewItemTemplate _ViewItemTemplate)
        {
            return new ViewItemTemplateCRUDViewModel
            {
                Id = _ViewItemTemplate.Id,
                //[INSERT_FIELD_MODEL_VIEW_IMPLICIT_1]
                CreatedDate = _ViewItemTemplate.CreatedDate,
                ModifiedDate = _ViewItemTemplate.ModifiedDate,
                CreatedBy = _ViewItemTemplate.CreatedBy,
                ModifiedBy = _ViewItemTemplate.ModifiedBy,
                Cancelled = _ViewItemTemplate.Cancelled,
            };
        }

        public static implicit operator ViewItemTemplate(ViewItemTemplateCRUDViewModel vm)
        {
            return new ViewItemTemplate
            {
                Id = vm.Id,
                //[INSERT_FIELD_MODEL_VIEW_IMPLICIT_2]
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
