using System.ComponentModel.DataAnnotations;
using AMS.Models;
using AMS.Models.NhapThanhPhamViewModel;

namespace AMS.Models.XuatThanhPhamViewModel
{
    public class XuatThanhPhamCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Ngày xuất")]
        public DateTime NgayXuat { get; set; }
        [Display(Name = "Đơn vị tiền tệ")]
        public string DonViTienTe { get; set; }

        public List<ThanhPhamXuatKhoCRUDViewModel> ThanhPhamXuatKhoList { get; set; }

        public static implicit operator XuatThanhPhamCRUDViewModel(XuatThanhPham _XuatThanhPham)
        {
            return new XuatThanhPhamCRUDViewModel
            {
                Id = _XuatThanhPham.Id,
                NgayXuat = _XuatThanhPham.NgayXuat,
                DonViTienTe = _XuatThanhPham.DonViTienTe,
                CreatedDate = _XuatThanhPham.CreatedDate,
                ModifiedDate = _XuatThanhPham.ModifiedDate,
                CreatedBy = _XuatThanhPham.CreatedBy,
                ModifiedBy = _XuatThanhPham.ModifiedBy,
                Cancelled = _XuatThanhPham.Cancelled,
            };
        }

        public static implicit operator XuatThanhPham(XuatThanhPhamCRUDViewModel vm)
        {
            return new XuatThanhPham
            {
                Id = vm.Id,
                NgayXuat = vm.NgayXuat,
                DonViTienTe = vm.DonViTienTe,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
