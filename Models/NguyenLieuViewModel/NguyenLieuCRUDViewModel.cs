using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.NguyenLieuViewModel
{
    public class NguyenLieuCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Mã Nguyên Liệu")]
        public string MaNguyenLieu { get; set; }


        [Display(Name = "Tên Nguyên Liệu")]
        public string TenNguyenLieu { get; set; }


        [Display(Name = "Số Lượng")]
        public int SoLuong { get; set; }

        [Display(Name = "Số Lượng Lý Thuyết")]
        public int SoLuongLyThuyet { get; set; }

        [Display(Name = "Đơn vị tính")]
        public string DonViTinh { get; set; }

        [Display(Name = "Thuộc kho")]
        public int KhoId { get; set; }

        [Display(Name = "Chi tiết custom nguyên liệu")]
        public List<AMS.Models.CustomFieldTotalViewModel.CustomFieldTotalCRUDViewModel>? ListCustom { get; set; }


        public int SoLuongList { get; set; }

        public static implicit operator NguyenLieuCRUDViewModel(NguyenLieu _NguyenLieu)
        {
            return new NguyenLieuCRUDViewModel
            {
                Id = _NguyenLieu.Id,
                MaNguyenLieu = _NguyenLieu.MaNguyenLieu,
                TenNguyenLieu = _NguyenLieu.TenNguyenLieu,
                SoLuongLyThuyet = _NguyenLieu.SoLuongLyThuyet,
                SoLuong = _NguyenLieu.SoLuong,
                DonViTinh = _NguyenLieu.DonViTinh,
                KhoId = _NguyenLieu.KhoId,
                CreatedDate = _NguyenLieu.CreatedDate,
                ModifiedDate = _NguyenLieu.ModifiedDate,
                CreatedBy = _NguyenLieu.CreatedBy,
                ModifiedBy = _NguyenLieu.ModifiedBy,
                Cancelled = _NguyenLieu.Cancelled,
            };
        }

        public static implicit operator NguyenLieu(NguyenLieuCRUDViewModel vm)
        {
            return new NguyenLieu
            {
                Id = vm.Id,
                MaNguyenLieu = vm.MaNguyenLieu,
                TenNguyenLieu = vm.TenNguyenLieu,
                SoLuongLyThuyet = vm.SoLuongLyThuyet,
                SoLuong = vm.SoLuong,
                DonViTinh = vm.DonViTinh,
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
