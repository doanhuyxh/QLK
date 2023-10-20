using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.XuatKhoViewModel
{
    public class XuatKhoCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Tên phiếu xuất kho")]
        public string TenPhieuXuatKho { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Đánh giá")]
        public string DanhGia { get; set; }
        [Display(Name = "Ngày xuất")]
        public DateTime NgayXuat { get; set; }
        [Display(Name = "Danh sách nguyên liệu xuất")]
        public List<NguyenLieuXuatKhoCRUDViewModel.NguyenLieuXuatKhoCRUDViewModel> ListNguyenLieuXuatKho { get; set; }


        public static implicit operator XuatKhoCRUDViewModel(XuatKho _XuatKho)
        {
            return new XuatKhoCRUDViewModel
            {
                Id = _XuatKho.Id,
                TenPhieuXuatKho = _XuatKho.TenPhieuXuatKho,                
                DanhGia = _XuatKho.DanhGia,
                CreatedDate = _XuatKho.CreatedDate,
                ModifiedDate = _XuatKho.ModifiedDate,
                CreatedBy = _XuatKho.CreatedBy,
                ModifiedBy = _XuatKho.ModifiedBy,
                Cancelled = _XuatKho.Cancelled,
                Status = _XuatKho.Status
            };
        }

        public static implicit operator XuatKho(XuatKhoCRUDViewModel vm)
        {
            return new XuatKho
            {
                Id = vm.Id,
                TenPhieuXuatKho = vm.TenPhieuXuatKho,
                DanhGia = vm.DanhGia,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
                Status = vm.Status
            };
        }
    }
}
