using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.CustomFieldViewModel
{
    public class CustomFieldCRUDViewModel
    {
        public int ID { get; set; }
        public string FieldName { get; set; }

        public static implicit operator CustomFieldCRUDViewModel(CustomField _Customproduct)
        {
            return new CustomFieldCRUDViewModel
            {
                ID = _Customproduct.ID,
                FieldName = _Customproduct.FieldName,

            };
        }

        public static implicit operator CustomField(CustomFieldCRUDViewModel vm)
        {
            return new CustomField
            {
                ID = vm.ID,
                FieldName = vm.FieldName,
            };
        }
    }
}
