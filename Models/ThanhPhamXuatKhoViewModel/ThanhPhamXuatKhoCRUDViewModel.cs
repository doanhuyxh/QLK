using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.NhapThanhPhamViewModel
{
    public class ThanhPhamXuatKhoCRUDViewModel : EntityBase
    {
        public int Id { get; set; }
        public int ThanhPhamId { get; set; }
        public int XuatKhoThanhPhamId { get; set; }
        public int SoLuong { get; set; }
        public int DonGia { get; set; }
        public string PO { get; set; }
        public string Size { get; set; }
        public string Mau { get; set; }
        public DateTime NgayXuat { get; set; }
        public string KhachHang { get; set; }
        public string MaHang { get; set; }

        public static implicit operator ThanhPhamXuatKhoCRUDViewModel(ThanhPhamXuatKho _XuatThanhPham)
        {
            return new ThanhPhamXuatKhoCRUDViewModel
            {
                Id = _XuatThanhPham.Id,
                ThanhPhamId = _XuatThanhPham.ThanhPhamId,
                XuatKhoThanhPhamId = _XuatThanhPham.XuatKhoThanhPhamId,
                SoLuong = _XuatThanhPham.SoLuong,
                DonGia = _XuatThanhPham.DonGia,
                PO = _XuatThanhPham.PO,
                Size = _XuatThanhPham.Size,
                Mau = _XuatThanhPham.Mau,
                CreatedDate = _XuatThanhPham.CreatedDate,
                ModifiedDate = _XuatThanhPham.ModifiedDate,
                CreatedBy = _XuatThanhPham.CreatedBy,
                ModifiedBy = _XuatThanhPham.ModifiedBy,
                Cancelled = _XuatThanhPham.Cancelled,
                NgayXuat = _XuatThanhPham.NgayXuat,
                KhachHang = _XuatThanhPham.KhachHang,
                MaHang = _XuatThanhPham.MaHang,
            };
        }

        public static implicit operator ThanhPhamXuatKho(ThanhPhamXuatKhoCRUDViewModel vm)
        {
            return new ThanhPhamXuatKho
            {
                Id = vm.Id,
                ThanhPhamId = vm.ThanhPhamId,
                XuatKhoThanhPhamId = vm.XuatKhoThanhPhamId,
                SoLuong = vm.SoLuong,
                DonGia = vm.DonGia,
                PO = vm.PO,
                Size = vm.Size,
                Mau = vm.Mau,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
                NgayXuat = vm.NgayXuat,
                KhachHang = vm.KhachHang,
                MaHang = vm.MaHang,
            };
        }
    }
}
