using System.ComponentModel.DataAnnotations;
using AMS.Models;
//[INSERT_IMPORT_FIELD_MODEL_VIEW]
namespace AMS.Models.DanhSachConViewModel
{
    public class DanhSachConCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }
        
                                    [Display(Name = "Danh sách con")]
                                    public int TestView3Id { get; set; }
                                    

                                    [Display (Name = "Tên con 1")] 
                                    public string TenCon1 { get; set; }
                                

                                    [Display(Name = "Tên con 2")]
                                    public string? LoaiKhoDisplay { get; set; }
                                    

                                    [Display(Name = "Tên con 2")]
                                    public int LoaiKhoId { get; set; }
                                    
        public static implicit operator DanhSachConCRUDViewModel(DanhSachCon _DanhSachCon)
        {
            return new DanhSachConCRUDViewModel
            {
                Id = _DanhSachCon.Id,
                TestView3Id = _DanhSachCon.TestView3Id,
TenCon1 = _DanhSachCon.TenCon1,
LoaiKhoId = _DanhSachCon.LoaiKhoId,
                CreatedDate = _DanhSachCon.CreatedDate,
                ModifiedDate = _DanhSachCon.ModifiedDate,
                CreatedBy = _DanhSachCon.CreatedBy,
                ModifiedBy = _DanhSachCon.ModifiedBy,
                Cancelled = _DanhSachCon.Cancelled,
            };
        }

        public static implicit operator DanhSachCon(DanhSachConCRUDViewModel vm)
        {
            return new DanhSachCon
            {
                Id = vm.Id,
                TestView3Id = vm.TestView3Id,
TenCon1 = vm.TenCon1,
LoaiKhoId = vm.LoaiKhoId,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
