using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.CustomproductViewModel
{
    public class CustomproductCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "InforProduct")]
        public string Inforproduct { get; set; }


        [Display(Name = "ParentId")]
        public int Parentid { get; set; }

        public static implicit operator CustomproductCRUDViewModel(Customproduct _Customproduct)
        {
            return new CustomproductCRUDViewModel
            {
                Id = _Customproduct.Id,
                Inforproduct = _Customproduct.Inforproduct,
                Parentid = _Customproduct.Parentid,
                CreatedDate = _Customproduct.CreatedDate,
                ModifiedDate = _Customproduct.ModifiedDate,
                CreatedBy = _Customproduct.CreatedBy,
                ModifiedBy = _Customproduct.ModifiedBy,
                Cancelled = _Customproduct.Cancelled,
            };
        }

        public static implicit operator Customproduct(CustomproductCRUDViewModel vm)
        {
            return new Customproduct
            {
                Id = vm.Id,
                Inforproduct = vm.Inforproduct,
                Parentid = vm.Parentid,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
