using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.NhapThanhPhamViewModel
{
    public class ThanhPhamNhapKhoCRUDViewModel : EntityBase
    {
        public int Id { get; set; }
        public int ThanhPhamId { get; set; }
        public int NhapKhoThanhPhamId { get; set; }
        public int SoLuong { get; set; }
        public int DonGia { get; set; }
        public string PO { get; set; }
        public string Size { get; set; }
        public string Mau { get; set; }
        public string KhachHang { get; set; }
        public string MaHang { get; set; }
        public DateTime NgayNhap { get; set; }

        public static implicit operator ThanhPhamNhapKhoCRUDViewModel(ThanhPhamNhapKho _NhapThanhPham)
        {
            return new ThanhPhamNhapKhoCRUDViewModel
            {
                Id = _NhapThanhPham.Id,
                ThanhPhamId = _NhapThanhPham.ThanhPhamId,
                NhapKhoThanhPhamId = _NhapThanhPham.NhapKhoThanhPhamId,
                SoLuong = _NhapThanhPham.SoLuong,
                DonGia = _NhapThanhPham.DonGia,
                PO = _NhapThanhPham.PO,
                Size = _NhapThanhPham.Size,
                Mau = _NhapThanhPham.Mau,
                KhachHang = _NhapThanhPham.KhachHang,
                MaHang = _NhapThanhPham.MaHang,
                NgayNhap = _NhapThanhPham.NgayNhap,
                CreatedDate = _NhapThanhPham.CreatedDate,
                ModifiedDate = _NhapThanhPham.ModifiedDate,
                CreatedBy = _NhapThanhPham.CreatedBy,
                ModifiedBy = _NhapThanhPham.ModifiedBy,
                Cancelled = _NhapThanhPham.Cancelled,
            };
        }

        public static implicit operator ThanhPhamNhapKho(ThanhPhamNhapKhoCRUDViewModel vm)
        {
            return new ThanhPhamNhapKho
            {
                Id = vm.Id,
                ThanhPhamId = vm.ThanhPhamId,
                NhapKhoThanhPhamId = vm.NhapKhoThanhPhamId,
                SoLuong = vm.SoLuong,
                DonGia = vm.DonGia,
                PO = vm.PO,
                Size = vm.Size,
                Mau = vm.Mau,
                KhachHang = vm.KhachHang,
                MaHang = vm.MaHang,
                NgayNhap = vm.NgayNhap,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
