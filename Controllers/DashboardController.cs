using AMS.Data;
using AMS.Models;
using AMS.Models.DashboardViewModel;
using AMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;

namespace AMS.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        public DashboardController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }
        public IActionResult Index()
        {
            ViewBag.SoLuongNguyenLieu = _context.NguyenLieu.Count(x => x.Cancelled == false);
            ViewBag.SoLuongNguoiDung = _context.UserProfile.Count(x => x.Cancelled == false);
            ViewBag.SoLuongKhachHang = _context.KhachHang.Count(x => x.Cancelled == false);
            return View();
        }

        public IActionResult CharNguyenLieuAPI()
        {
            var result = (from _nl in _context.NguyenLieu
                          where _nl.Cancelled == false
                          select new dataChartNL
                          {
                              label = _nl.TenNguyenLieu,
                              TT = _nl.SoLuong,
                              LT = _nl.SoLuongLyThuyet,
                          }).ToList();

            return Ok(result);
        }

        public IActionResult DanhSachNhapHangChoLyThuyet()
        {
            var ds = (from _nk in _context.NhapKhoLyThuyet
                      where _nk.Cancelled == false && _nk.Status == false
                      select new
                      {
                          MaPhieu = _nk.MaPhieu,
                          SoLo = _nk.MaSoLo,
                          TenKhachHang = _nk.TenKhachHang,
                          NgayNhap = _nk.DuKienNgayVe,
                          TrangThai = _nk.Status,
                      }
                      ).OrderByDescending(x=>x.NgayNhap).ToList();

            return Ok(ds.Take(10));
        }

        public IActionResult DanhSachUserTaoGanDay()
        {
            var dsUser = (from _us in _context.UserProfile
                          where _us.Cancelled == false && _us.Email != "admin@gmail.com"
                          select new
                          {
                              Name = $"{_us.FirstName} {_us.LastName}",
                              AnhDaiDien = _us.ProfilePicture,
                              QuyenHan = (_context.QuanLyNhomNguoiDung.SingleOrDefault(x => x.Id == _us.GroupUserId)) == null ? "" : (_context.QuanLyNhomNguoiDung.SingleOrDefault(x => x.Id == _us.GroupUserId).TenNhom),
                              NgaySuaCuoi = _us.ModifiedDate.ToString("dd/MM/yyyy"),
                          }).ToList();

            dsUser = dsUser.Count >= 5 ? dsUser.GetRange(dsUser.Count - 5, 5) : dsUser;

            return Ok(dsUser);
        }
    }
    public class dataChartNL
    {
        public string label { get; set; }
        public int TT { get; set; }
        public int LT { get; set; }
    }
}