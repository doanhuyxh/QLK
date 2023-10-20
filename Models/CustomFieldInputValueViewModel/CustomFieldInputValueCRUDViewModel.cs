using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.CustomFieldInputValueViewModel
{
    public class CustomFieldInputValueCRUDViewModel
    {
        public int ID { get; set; }
        public int CustomFieldKey { get; set; }
        public string CustomFieldValue { get; set; }

        public static implicit operator CustomFieldInputValueCRUDViewModel(CustomFieldInputValue _Customproduct)
        {
            return new CustomFieldInputValueCRUDViewModel
            {
                ID = _Customproduct.ID,
                CustomFieldKey = _Customproduct.CustomFieldKey,
                CustomFieldValue = _Customproduct.CustomFieldValue,

            };
        }

        public static implicit operator CustomFieldInputValue(CustomFieldInputValueCRUDViewModel vm)
        {
            return new CustomFieldInputValue
            {
                ID = vm.ID,
                CustomFieldKey = vm.CustomFieldKey,
                CustomFieldValue = vm.CustomFieldValue,

            };
        }
    }
}
