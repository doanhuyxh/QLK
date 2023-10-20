using System.ComponentModel.DataAnnotations;
using AMS.Models;
using AMS.Models.ListNhapThanhPhamViewModel;
using AMS.Models.NhapThanhPhamViewModel;

namespace AMS.Models.PhieuNhapThanhPhamViewModel
{
    public class PhieuNhapThanhPhamCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Mã nhập")]
        public string MaNhap { get; set; }
        [Display(Name = "Ngày dự kiến nhập")]

        public DateTime NgayDuKienNhap { get; set; }
        [Display(Name = "Đơn vị tiền tệ")]
        public string DonViTienTe { get; set; }

        public List<ListNhapThanhPhamCRUDViewModel> ThanhPhamNhapKhoList { get; set; }


        public static implicit operator PhieuNhapThanhPhamCRUDViewModel(PhieuNhapThanhPham _PhieuNhapThanhPham)
        {
            return new PhieuNhapThanhPhamCRUDViewModel
            {
                Id = _PhieuNhapThanhPham.Id,
                MaNhap = _PhieuNhapThanhPham.MaNhap,
                CreatedDate = _PhieuNhapThanhPham.CreatedDate,
                ModifiedDate = _PhieuNhapThanhPham.ModifiedDate,
                CreatedBy = _PhieuNhapThanhPham.CreatedBy,
                ModifiedBy = _PhieuNhapThanhPham.ModifiedBy,
                Cancelled = _PhieuNhapThanhPham.Cancelled,
                NgayDuKienNhap = _PhieuNhapThanhPham.NgayDuKienNhap,
                DonViTienTe = _PhieuNhapThanhPham.DonViTienTe,
            };
        }

        public static implicit operator PhieuNhapThanhPham(PhieuNhapThanhPhamCRUDViewModel vm)
        {
            return new PhieuNhapThanhPham
            {
                Id = vm.Id,
                MaNhap = vm.MaNhap,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
                NgayDuKienNhap = vm.NgayDuKienNhap,
                DonViTienTe = vm.DonViTienTe,
            };
        }
    }
}
