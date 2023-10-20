using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.ThongBaoChenhLechNguyenLieuViewModel
{
    public class ThongBaoChenhLechNguyenLieuCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }
        
                                    [Display (Name = "Nội dung")] 
                                    public string NoiDung { get; set; }
                                
        public static implicit operator ThongBaoChenhLechNguyenLieuCRUDViewModel(ThongBaoChenhLechNguyenLieu _ThongBaoChenhLechNguyenLieu)
        {
            return new ThongBaoChenhLechNguyenLieuCRUDViewModel
            {
                Id = _ThongBaoChenhLechNguyenLieu.Id,
                NoiDung = _ThongBaoChenhLechNguyenLieu.NoiDung,
                CreatedDate = _ThongBaoChenhLechNguyenLieu.CreatedDate,
                ModifiedDate = _ThongBaoChenhLechNguyenLieu.ModifiedDate,
                CreatedBy = _ThongBaoChenhLechNguyenLieu.CreatedBy,
                ModifiedBy = _ThongBaoChenhLechNguyenLieu.ModifiedBy,
                Cancelled = _ThongBaoChenhLechNguyenLieu.Cancelled,
            };
        }

        public static implicit operator ThongBaoChenhLechNguyenLieu(ThongBaoChenhLechNguyenLieuCRUDViewModel vm)
        {
            return new ThongBaoChenhLechNguyenLieu
            {
                Id = vm.Id,
                NoiDung = vm.NoiDung,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
