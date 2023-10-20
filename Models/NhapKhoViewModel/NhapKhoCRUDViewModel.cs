using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.NhapKhoViewModel
{
    public class NhapKhoCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }
        [Display(Name = "Ngày nhập")]
        public DateTime NgayNhap { get; set; }
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }
        [Display(Name = "Đơn vị tiền tệ")]
        public string? DonViTienTe { get; set; }
        [Display(Name ="Danh sách nguyên liệu nhập kho")]
        public List<AMS.Models.NguyenLieuNhapKhoCRUDViewModel.NguyenLieuNhapKhoCRUDViewModel>? ListNguyenLieuNhapKho { get; set; }

        public static implicit operator NhapKhoCRUDViewModel(NhapKho _NhapKho)
        {
            return new NhapKhoCRUDViewModel
            {
                Id = _NhapKho.Id,
                NgayNhap = _NhapKho.NgayNhap,
                MoTa = _NhapKho.MoTa,
                DonViTienTe = _NhapKho.DonViTienTe,
                CreatedDate = _NhapKho.CreatedDate,
                ModifiedDate = _NhapKho.ModifiedDate,
                CreatedBy = _NhapKho.CreatedBy,
                ModifiedBy = _NhapKho.ModifiedBy,
                Cancelled = _NhapKho.Cancelled,
            };
        }

        public static implicit operator NhapKho(NhapKhoCRUDViewModel vm)
        {
            return new NhapKho
            {
                Id = vm.Id,
                NgayNhap = vm.NgayNhap,
                MoTa = vm.MoTa??"",
                DonViTienTe = vm.DonViTienTe ?? "",
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
