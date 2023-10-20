using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.XayDungCongThucViewModel
{
    public class XayDungCongThucCRUDViewModels : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int? Id { get; set; }

        [Display(Name = "Tên sản phẩm")]
        [Required(ErrorMessage = "Tên thành phẩm không được để trống")]
        public string Name { get; set; }

        [Display(Name = "Mô tả chi tiết sản phẩm")]
        [Required(ErrorMessage = "Mô tả thành phẩm không được để trống")]
        public string Description { get; set; }

        [Display(Name = "Mã sản phẩm")]
        [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
        public string MaSP { get; set; }

        [Display(Name ="Tên nguyên liệu")]
        public string? TenNguyenLieuDisplay { get; set; }

        public List<ListNguyenLieuInCongThucViewModel.ListNguyenLieuInCongThucViewModel>? ListNguyenLieuInCongThuc { get; set; }


        public static implicit operator XayDungCongThucCRUDViewModels(XayDungCongThuc _DanhSachXayDungCongThuc)
        {
            return new XayDungCongThucCRUDViewModels
            {
                Id = _DanhSachXayDungCongThuc.Id,
                Name = _DanhSachXayDungCongThuc.Name,
                Description = _DanhSachXayDungCongThuc.Description,
                MaSP = _DanhSachXayDungCongThuc.MaSP,
                CreatedDate = _DanhSachXayDungCongThuc.CreatedDate,
                ModifiedDate = _DanhSachXayDungCongThuc.ModifiedDate,
                CreatedBy = _DanhSachXayDungCongThuc.CreatedBy,
                ModifiedBy = _DanhSachXayDungCongThuc.ModifiedBy,
                Cancelled = _DanhSachXayDungCongThuc.Cancelled,
            };
        }

        public static implicit operator XayDungCongThuc(XayDungCongThucCRUDViewModels vm)
        {
            return new XayDungCongThuc
            {
                Id = vm.Id ?? 0,
                Name = vm.Name,
                Description = vm.Description,
                MaSP = vm.MaSP,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
