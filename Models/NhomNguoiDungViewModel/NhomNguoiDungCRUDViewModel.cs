using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.NhomNguoiDungViewModel
{
    public class NhomNguoiDungCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Tên nhóm")]
        public string TenNhom { get; set; }


        [Display(Name = "Nhóm cha")]
        public int ParentId { get; set; }

        [Display(Name = "Nhóm cha")]
        public string? ParentName { get; set; }


        [Display(Name = "Quyền hạn nhóm")]
        public string QuyenHanNhom { get; set; }


        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }


        [Display(Name = "Ảnh đại diện")]
        public string? AnhDaiDienPath { get; set; }


        [Display(Name = "Ảnh đại diện")]
        public IFormFile AnhDaiDien { get; set; }

        public static implicit operator NhomNguoiDungCRUDViewModel(NhomNguoiDung _NhomNguoiDung)
        {
            return new NhomNguoiDungCRUDViewModel
            {
                Id = _NhomNguoiDung.Id,
                TenNhom = _NhomNguoiDung.TenNhom,
                ParentId = _NhomNguoiDung.ParentId,
                QuyenHanNhom = _NhomNguoiDung.QuyenHanNhom,
                MoTa = _NhomNguoiDung.MoTa,
                AnhDaiDienPath = _NhomNguoiDung.AnhDaiDienPath,
                CreatedDate = _NhomNguoiDung.CreatedDate,
                ModifiedDate = _NhomNguoiDung.ModifiedDate,
                CreatedBy = _NhomNguoiDung.CreatedBy,
                ModifiedBy = _NhomNguoiDung.ModifiedBy,
                Cancelled = _NhomNguoiDung.Cancelled,
            };
        }

        public static implicit operator NhomNguoiDung(NhomNguoiDungCRUDViewModel vm)
        {
            return new NhomNguoiDung
            {
                Id = vm.Id,
                TenNhom = vm.TenNhom,
                ParentId = vm.ParentId,
                QuyenHanNhom = vm.QuyenHanNhom,
                MoTa = vm.MoTa,
                AnhDaiDienPath = vm.AnhDaiDienPath,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
