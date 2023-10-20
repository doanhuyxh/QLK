using System.ComponentModel.DataAnnotations;
using AMS.Models;
using AMS.Models.ListXuatThanhPhamViewModel;

namespace AMS.Models.PhieuXuatThanhPhamViewModel
{
    public class PhieuXuatThanhPhamCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Ma Phieu Xuat")]
        public string MaPhieuXuat { get; set; }


        [Display(Name = "Ngay Du Kien Xuat")]
        public DateTime NgayDuKienXuat { get; set; }

        public List<ListXuatThanhPhamCRUDViewModel> ThanhPhamNhapKhoList { get; set; }



        public static implicit operator PhieuXuatThanhPhamCRUDViewModel(PhieuXuatThanhPham _PhieuXuatThanhPham)
        {
            return new PhieuXuatThanhPhamCRUDViewModel
            {
                Id = _PhieuXuatThanhPham.Id,
                MaPhieuXuat = _PhieuXuatThanhPham.MaPhieuXuat,
                NgayDuKienXuat = _PhieuXuatThanhPham.NgayDuKienXuat,
                CreatedDate = _PhieuXuatThanhPham.CreatedDate,
                ModifiedDate = _PhieuXuatThanhPham.ModifiedDate,
                CreatedBy = _PhieuXuatThanhPham.CreatedBy,
                ModifiedBy = _PhieuXuatThanhPham.ModifiedBy,
                Cancelled = _PhieuXuatThanhPham.Cancelled,
            };
        }

        public static implicit operator PhieuXuatThanhPham(PhieuXuatThanhPhamCRUDViewModel vm)
        {
            return new PhieuXuatThanhPham
            {
                Id = vm.Id,
                MaPhieuXuat = vm.MaPhieuXuat,
                NgayDuKienXuat = vm.NgayDuKienXuat,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
