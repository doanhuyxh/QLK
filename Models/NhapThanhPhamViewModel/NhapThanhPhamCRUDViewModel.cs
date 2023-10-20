using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.NhapThanhPhamViewModel
{
    public class NhapThanhPhamCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Ngày nhập")]
        public DateTime NgayNhap { get; set; }
        [Display(Name = "Mã Nhập")]
        public string Ma { get; set; } = string.Empty;
        [Display(Name = "Đơn vị tiền tệ")]
        public string DonViTienTe { get; set; }

        [Display(Name = "Thành phẩm nhập kho")]
        [Required(ErrorMessage = "Cần phải có thành phẩm")]
        public List<ThanhPhamNhapKhoCRUDViewModel> ThanhPhamNhapKhoList { get; set; }

        public static implicit operator NhapThanhPhamCRUDViewModel(NhapThanhPham _NhapThanhPham)
        {
            return new NhapThanhPhamCRUDViewModel
            {
                Id = _NhapThanhPham.Id,
                Ma = _NhapThanhPham.Ma,
                DonViTienTe = _NhapThanhPham.DonViTienTe,
                NgayNhap = _NhapThanhPham.NgayNhap,
                CreatedDate = _NhapThanhPham.CreatedDate,
                ModifiedDate = _NhapThanhPham.ModifiedDate,
                CreatedBy = _NhapThanhPham.CreatedBy,
                ModifiedBy = _NhapThanhPham.ModifiedBy,
                Cancelled = _NhapThanhPham.Cancelled,
            };
        }

        public static implicit operator NhapThanhPham(NhapThanhPhamCRUDViewModel vm)
        {
            return new NhapThanhPham
            {
                Id = vm.Id,
                Ma = vm.Ma,
                DonViTienTe = vm.DonViTienTe ?? "",
                NgayNhap = vm.NgayNhap,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
