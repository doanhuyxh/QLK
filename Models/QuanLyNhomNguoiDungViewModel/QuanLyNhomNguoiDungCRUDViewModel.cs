using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.QuanLyNhomNguoiDungViewModel
{
    public class QuanLyNhomNguoiDungCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Tên nhóm")]
        public string TenNhom { get; set; }

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Quyền hạn")]
        public string Permistions { get; set; }
        public List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, Access>>>>>>? ViewSectionAccessList { get; set; }
        public static implicit operator QuanLyNhomNguoiDungCRUDViewModel(QuanLyNhomNguoiDung _QuanLyNhomNguoiDung)
        {
            return new QuanLyNhomNguoiDungCRUDViewModel
            {
                Id = _QuanLyNhomNguoiDung.Id,
                TenNhom = _QuanLyNhomNguoiDung.TenNhom,
                MoTa = _QuanLyNhomNguoiDung.MoTa,
                CreatedDate = _QuanLyNhomNguoiDung.CreatedDate,
                Permistions = _QuanLyNhomNguoiDung.Permistions,
                ModifiedDate = _QuanLyNhomNguoiDung.ModifiedDate,
                CreatedBy = _QuanLyNhomNguoiDung.CreatedBy,
                ModifiedBy = _QuanLyNhomNguoiDung.ModifiedBy,
                Cancelled = _QuanLyNhomNguoiDung.Cancelled,
            };
        }

        public static implicit operator QuanLyNhomNguoiDung(QuanLyNhomNguoiDungCRUDViewModel vm)
        {
            return new QuanLyNhomNguoiDung
            {
                Id = vm.Id,
                TenNhom = vm.TenNhom,
                MoTa = vm.MoTa??string.Empty,
                CreatedDate = vm.CreatedDate,
                Permistions = vm.Permistions,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
