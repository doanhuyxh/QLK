using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.KeHoachSanXuatViewModel
{
    public class KeHoachSanXuatCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Tên kế hoạch")]
        public string TenThanhPham { get; set; }

        [Display(Name = "Ngày dự kiến hoàn thành")]
        public DateTime NgayDuKienHoan { get; set; }
        [Display(Name ="Số lượng thành phẩm")]
        public int? SoLuongThanhPham { get; set; }
        public List<AMS.Models.NguyenLieuKeHoachCRUDViewModel.NguyenLieuKeHoachCRUDViewModel> ListNguyenLieuKeHoach { get; set; }


        public static implicit operator KeHoachSanXuatCRUDViewModel(KeHoachSanXuat _KeHoachSanXuat)
        {
            return new KeHoachSanXuatCRUDViewModel
            {
                Id = _KeHoachSanXuat.Id,
                TenThanhPham = _KeHoachSanXuat.TenThanhPham,
                NgayDuKienHoan = _KeHoachSanXuat.NgayDuKienHoan,
                SoLuongThanhPham = _KeHoachSanXuat.SoLuongThanhPham,
                CreatedDate = _KeHoachSanXuat.CreatedDate,
                ModifiedDate = _KeHoachSanXuat.ModifiedDate,
                CreatedBy = _KeHoachSanXuat.CreatedBy,
                ModifiedBy = _KeHoachSanXuat.ModifiedBy,
                Cancelled = _KeHoachSanXuat.Cancelled,
            };
        }

        public static implicit operator KeHoachSanXuat(KeHoachSanXuatCRUDViewModel vm)
        {
            return new KeHoachSanXuat
            {
                Id = vm.Id,
                TenThanhPham = vm.TenThanhPham,
                NgayDuKienHoan = vm.NgayDuKienHoan,
                SoLuongThanhPham = vm.SoLuongThanhPham,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
