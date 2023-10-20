using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.CustomFieldTotalViewModel
{
    public class CustomFieldTotalCRUDViewModel
    {
        public int ID { get; set; }
        public int NguyenLieuID { get; set; }
        public string ListCustom { get; set; }
        public int QuantityProduct { get; set; }
        public bool Cancel { get; set; }


        public static implicit operator CustomFieldTotalCRUDViewModel(CustomFieldTotal _Customproduct)
        {
            return new CustomFieldTotal
            {
                ID = _Customproduct.ID,
                NguyenLieuID = _Customproduct.NguyenLieuID,
                ListCustom = _Customproduct.ListCustom,
                QuantityProduct = _Customproduct.QuantityProduct,
                Cancel = _Customproduct.Cancel,

            };
        }

        public static implicit operator CustomFieldTotal(CustomFieldTotalCRUDViewModel vm)
        {
            return new CustomFieldTotal
            {
                ID = vm.ID,
                NguyenLieuID = vm.NguyenLieuID,
                ListCustom = vm.ListCustom,
                QuantityProduct = vm.QuantityProduct,
                Cancel = vm.Cancel,

            };
        }
    }
}
