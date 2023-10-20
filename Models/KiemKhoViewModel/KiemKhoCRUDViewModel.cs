using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.KiemKhoViewModel
{
    public class KiemKhoCRUDViewModel : EntityBase
    {

        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }
        [Display(Name = "Tên phiếu")]
        [Required(ErrorMessage = "Tên phiếu không được để trống")]
        public string Name { get; set; }
        [Display(Name = "Ngày kiểm kho")]
        [Required(ErrorMessage = "ngày kiểm phiếu không hợp lệ")]
        public DateTime NgayKiem { get; set; }
        [Display(Name = "Kho")]
        [Required(ErrorMessage = "Phải điền kho kiểm")]
        public int? KhoId { get; set; }
        [Display(Name = "Nhân viên chịu trách nhiệm")]
        [Required(ErrorMessage = "Chưa chọn nhân viên kiểm kho")]
        public int NhanVienId { get; set; }
        public List<NguyenLieuKiemKhoViewModel.NguyenLieuKiemKhoViewModel>? NguyenLieuKiemKho { get; set; }


        public static implicit operator KiemKhoCRUDViewModel(KiemKho _KiemKho)
        {
            return new KiemKhoCRUDViewModel
            {
                Id = _KiemKho.Id,
                Name = _KiemKho.Name,
                NgayKiem = _KiemKho.NgayKiem,
                KhoId = _KiemKho.KhoId,
                NhanVienId = _KiemKho.NhanVienId,
                CreatedDate = _KiemKho.CreatedDate,
                ModifiedDate = _KiemKho.ModifiedDate,
                CreatedBy = _KiemKho.CreatedBy,
                ModifiedBy = _KiemKho.ModifiedBy,
                Cancelled = _KiemKho.Cancelled,
            };
        }

        public static implicit operator KiemKho(KiemKhoCRUDViewModel vm)
        {
            return new KiemKho
            {
                Id = vm.Id,
                Name = vm.Name,
                NgayKiem = vm.NgayKiem,
                NhanVienId = vm.NhanVienId,
                KhoId = vm.KhoId,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
