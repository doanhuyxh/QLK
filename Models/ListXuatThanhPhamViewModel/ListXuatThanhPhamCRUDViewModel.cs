using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.ListXuatThanhPhamViewModel
{
    public class ListXuatThanhPhamCRUDViewModel : EntityBase
    {
        public int ID { get; set; }
        public int IDPhieu { get; set; }
        public int IDThanhPham { get; set; }
        public string? PO { get; set; }
        public string? Mau { get; set; }
        public string? Size { get; set; }
        public int SoLuong { get; set; }
        public int DonGia { get; set; }

        public string? TenNguyenLieuByPhieuNhap { get; set; }


        public string KhachHang { get; set; }
        public string MaHang { get; set; }
        public DateTime NgayNhap { get; set; }


        public static implicit operator ListXuatThanhPhamCRUDViewModel(ListXuatThanhPham _NguyenLieu)
        {
            return new ListXuatThanhPhamCRUDViewModel
            {
                ID = _NguyenLieu.ID,
                IDPhieu = _NguyenLieu.IDPhieu,
                IDThanhPham = _NguyenLieu.IDThanhPham,
                PO = _NguyenLieu.PO,
                Mau = _NguyenLieu.Mau,
                CreatedBy = _NguyenLieu.CreatedBy,
                CreatedDate = _NguyenLieu.CreatedDate,
                ModifiedBy = _NguyenLieu.ModifiedBy,
                ModifiedDate = _NguyenLieu.ModifiedDate,
                Cancelled = _NguyenLieu.Cancelled,
                Size = _NguyenLieu.Size,
                SoLuong = _NguyenLieu.SoLuong,
                DonGia = _NguyenLieu.DonGia,
                KhachHang = _NguyenLieu.KhachHang,
                MaHang = _NguyenLieu.MaHang,
                NgayNhap = _NguyenLieu.NgayNhap,
               
            };
        }

        public static implicit operator ListXuatThanhPham(ListXuatThanhPhamCRUDViewModel vm)
        {
            return new ListXuatThanhPham
            {
                ID = vm.ID,
                IDPhieu = vm.IDPhieu,
                IDThanhPham = vm.IDThanhPham,
                PO = vm.PO,
                Mau = vm.Mau,
                CreatedBy = vm.CreatedBy,
                CreatedDate = vm.CreatedDate,
                ModifiedBy = vm.ModifiedBy,
                ModifiedDate = vm.ModifiedDate,
                Cancelled = vm.Cancelled,
                Size = vm.Size,
                SoLuong = vm.SoLuong,
                DonGia = vm.DonGia,
                KhachHang = vm.KhachHang,
                MaHang = vm.MaHang,
                NgayNhap = vm.NgayNhap,
            };
        }
    }
}
