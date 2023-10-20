using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.TheoDoiChatLuongViewModel
{
    public class TheoDoiChatLuongCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Tên nguyên liệu")]
        public int TenNguyenLieuId { get; set; }
        [Display(Name = "Ghi chú về chất lượng")]
        public string? GhiChuVeChatLuong { get; set; }
        [Display(Name = "Đánh giá chất lượng")]

        public int ChatLuongDrop { get; set; }

        public string? DisplayName { get; set; }

        public static implicit operator TheoDoiChatLuongCRUDViewModel(TheoDoiChatLuong _TheoDoiChatLuong)
        {
            return new TheoDoiChatLuongCRUDViewModel
            {
                Id = _TheoDoiChatLuong.Id,
                TenNguyenLieuId = _TheoDoiChatLuong.TenNguyenLieuId,
                GhiChuVeChatLuong = _TheoDoiChatLuong.GhiChuVeChatLuong,
                CreatedDate = _TheoDoiChatLuong.CreatedDate,
                ModifiedDate = _TheoDoiChatLuong.ModifiedDate,
                CreatedBy = _TheoDoiChatLuong.CreatedBy,
                ModifiedBy = _TheoDoiChatLuong.ModifiedBy,
                Cancelled = _TheoDoiChatLuong.Cancelled,
                ChatLuongDrop = _TheoDoiChatLuong.ChatLuongDrop,
            };
        }

        public static implicit operator TheoDoiChatLuong(TheoDoiChatLuongCRUDViewModel vm)
        {
            return new TheoDoiChatLuong
            {
                Id = vm.Id,
                TenNguyenLieuId = vm.TenNguyenLieuId,
                GhiChuVeChatLuong = vm.GhiChuVeChatLuong,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
                ChatLuongDrop = vm.ChatLuongDrop,
            };
        }
    }
}
