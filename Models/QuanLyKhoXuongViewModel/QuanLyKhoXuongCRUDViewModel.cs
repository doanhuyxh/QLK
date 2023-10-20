using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.QuanLyKhoXuongViewModel
{
    public class QuanLyKhoXuongCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Tên Kho")]
        public string TenKho { get; set; }


        [Display(Name = "Ghi Chú")]
        public string GhiChu { get; set; }


        [Display(Name = "Thuộc kho cha")]
        public int ParentId { get; set; }

        public static implicit operator QuanLyKhoXuongCRUDViewModel(QuanLyKhoXuong _QuanLyKhoXuong)
        {
            return new QuanLyKhoXuongCRUDViewModel
            {
                Id = _QuanLyKhoXuong.Id,
                TenKho = _QuanLyKhoXuong.TenKho,
                GhiChu = _QuanLyKhoXuong.GhiChu,
                ParentId = _QuanLyKhoXuong.ParentId,
                CreatedDate = _QuanLyKhoXuong.CreatedDate,
                ModifiedDate = _QuanLyKhoXuong.ModifiedDate,
                CreatedBy = _QuanLyKhoXuong.CreatedBy,
                ModifiedBy = _QuanLyKhoXuong.ModifiedBy,
                Cancelled = _QuanLyKhoXuong.Cancelled,
            };
        }

        public static implicit operator QuanLyKhoXuong(QuanLyKhoXuongCRUDViewModel vm)
        {
            return new QuanLyKhoXuong
            {
                Id = vm.Id,
                TenKho = vm.TenKho,
                GhiChu = vm.GhiChu,
                ParentId = vm.ParentId,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
