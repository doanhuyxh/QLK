using System;

namespace AMS.Models
{
    public class TheoDoiChatLuong : EntityBase
    {
        public int Id { get; set; }
        public int TenNguyenLieuId { get; set; }
        public string? GhiChuVeChatLuong { get; set; }
        public int ChatLuongDrop { get; set; }
    }
}
