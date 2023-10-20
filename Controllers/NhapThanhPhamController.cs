using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.NhapThanhPhamViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
using AMS.Models.NguyenLieuViewModel;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class NhapThanhPhamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;

        public NhapThanhPhamController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name ?? string.Empty;
        }


        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapThanhPham.NhapThanhPham.list"))
            {
                return View();
            }
            else if (_iCommon.CheckPermissionInProfile(user, "NhapThanhPham.NhapThanhPham.list_owner"))
            {
                return View();
            }

            return View("_AccessDenied");

        }

        [HttpPost]
        public IActionResult GetDataTabelData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();

                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnAscDesc = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int resultTotal = 0;

                var _GetGridItem = GetGridItem();
                //NOT_IN_TREE_VIEW_START
                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnAscDesc)))
                {
                    _GetGridItem = _GetGridItem.OrderBy(sortColumn + " " + sortColumnAscDesc);
                }

                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    searchValue = searchValue.ToLower();
                    _GetGridItem = _GetGridItem.Where(obj => obj.Id.ToString().Contains(searchValue)
                    || obj.NgayNhap.ToString().ToLower().Contains(searchValue)
                    || obj.CreatedDate.ToString().ToLower().Contains(searchValue)
                    || obj.ModifiedDate.ToString().ToLower().Contains(searchValue)
                    || obj.CreatedBy.ToLower().Contains(searchValue)
                    || obj.ModifiedBy.ToLower().Contains(searchValue)

                    || obj.CreatedDate.ToString().Contains(searchValue));
                }
                //NOT_IN_TREE_VIEW_END
                resultTotal = _GetGridItem.Count();

                var result = _GetGridItem.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = resultTotal, recordsTotal = resultTotal, data = result });

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //GetGridItemStart
        private IQueryable<NhapThanhPhamCRUDViewModel> GetGridItem()
        {
            if (_iCommon.CheckPermissionInProfile(user, "NhapThanhPham.NhapThanhPham.list_owner"))
            {
                try
                {
                    return (from _NhapThanhPham in _context.NhapThanhPham
                            where _NhapThanhPham.Cancelled == false && _NhapThanhPham.CreatedBy.Contains(user)

                            select new NhapThanhPhamCRUDViewModel
                            {
                                Id = _NhapThanhPham.Id,
                                Ma = _NhapThanhPham.Ma,
                                DonViTienTe = _NhapThanhPham.DonViTienTe,
                                NgayNhap = _NhapThanhPham.NgayNhap,
                                CreatedDate = _NhapThanhPham.CreatedDate,
                                ModifiedDate = _NhapThanhPham.ModifiedDate,
                                CreatedBy = _NhapThanhPham.CreatedBy,
                                ModifiedBy = _NhapThanhPham.ModifiedBy,

                            }).OrderByDescending(x => x.Id);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            };


            try
            {
                return (from _NhapThanhPham in _context.NhapThanhPham
                        where _NhapThanhPham.Cancelled == false

                        select new NhapThanhPhamCRUDViewModel
                        {
                            Id = _NhapThanhPham.Id,
                            Ma = _NhapThanhPham.Ma,
                            DonViTienTe = _NhapThanhPham.DonViTienTe,
                            NgayNhap = _NhapThanhPham.NgayNhap,
                            CreatedDate = _NhapThanhPham.CreatedDate,
                            ModifiedDate = _NhapThanhPham.ModifiedDate,
                            CreatedBy = _NhapThanhPham.CreatedBy,
                            ModifiedBy = _NhapThanhPham.ModifiedBy,

                        }).OrderByDescending(x => x.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //GetGridItemEnd
        [HttpGet]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();
            NhapThanhPhamCRUDViewModel vm = await _context.NhapThanhPham.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            NhapThanhPhamCRUDViewModel vm = new NhapThanhPhamCRUDViewModel();

            if (id > 0)
            {
                vm = await _context.NhapThanhPham.Where(x => x.Id == id).SingleOrDefaultAsync();
                ViewBag.DanhSachThanhPham = JsonConvert.SerializeObject(GetThanhPhamNhapKhoByNhapThanhPhamId(id));
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapThanhPham.NhapThanhPham.edit"))
                {
                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "NhapThanhPham.NhapThanhPham.edit_owner"))
                {
                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapThanhPham.NhapThanhPham.create"))
                {
                    ViewBag.DanhSachThanhPham = JsonConvert.SerializeObject(Array.Empty<ThanhPhamNhapKhoCRUDViewModel>());
                    return PartialView("_AddEdit", vm);
                }
            }

            return View("_AccessDeniedAddEdit");

        }
        //[INSERT_LOAD]

        private List<ThanhPhamNhapKhoCRUDViewModel> GetThanhPhamNhapKhoByNhapThanhPhamId(int id)
        {
            var rs = from _tp in _context.ThanhPhamNhapKho
                     where _tp.NhapKhoThanhPhamId == id && _tp.Cancelled == false
                     select new ThanhPhamNhapKhoCRUDViewModel
                     {
                         Id = _tp.Id,
                         ThanhPhamId = _tp.ThanhPhamId,
                         SoLuong = _tp.SoLuong,
                         DonGia = _tp.DonGia,
                         Cancelled = _tp.Cancelled,
                         NhapKhoThanhPhamId = _tp.NhapKhoThanhPhamId,
                         PO = _tp.PO,
                         Size = _tp.Size,
                         Mau = _tp.Mau,
                         KhachHang = _tp.KhachHang,
                         MaHang = _tp.MaHang,
                         NgayNhap = _tp.NgayNhap,
                     };
            return rs.ToList();
        }

        [HttpPost]
        public async Task<JsonResult> AddEdit(NhapThanhPhamCRUDViewModel vm)
        {
            try
            {
                JsonResultViewModel _JsonResultViewModel = new();

                if (ModelState.IsValid)
                {
                    NhapThanhPham _NhapThanhPham = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _NhapThanhPham = await _context.NhapThanhPham.FindAsync(vm.Id);
                        vm.CreatedDate = _NhapThanhPham.CreatedDate;
                        vm.CreatedBy = _NhapThanhPham.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_NhapThanhPham).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        foreach (var item in vm.ThanhPhamNhapKhoList)
                        {
                            ThanhPhamNhapKho tp = new ThanhPhamNhapKho();

                            if (item.Id > 0)
                            {
                                tp = _context.ThanhPhamNhapKho.FirstOrDefault(x => x.Id == item.Id);
                                tp.Cancelled = item.Cancelled;
                                tp.SoLuong = item.SoLuong;
                                tp.DonGia = item.DonGia;
                                tp.Mau = item.Mau;
                                tp.Size = item.Size;
                                tp.PO = item.PO;
                                tp.ThanhPhamId = item.ThanhPhamId;
                                tp.KhachHang = item.KhachHang;
                                tp.MaHang = item.MaHang;
                                tp.NgayNhap = item.NgayNhap;
                                tp.ModifiedDate = DateTime.Now;
                                tp.ModifiedBy = _UserName;
                                _context.Update(tp);
                                await _context.SaveChangesAsync();

                                QuanLyThanhPham tp2 = new QuanLyThanhPham();
                                tp2 = _context.QuanLyThanhPham.FirstOrDefault(i => i.Id == tp.ThanhPhamId);
                                tp2.SoLuong += tp.SoLuong;
                                _context.Update(tp2);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                tp.Id = item.Id;
                                tp.NhapKhoThanhPhamId = _NhapThanhPham.Id;
                                tp.SoLuong = item.SoLuong;
                                tp.DonGia = item.DonGia;
                                tp.Cancelled = item.Cancelled;
                                tp.ThanhPhamId = item.ThanhPhamId;
                                tp.PO = item.PO;
                                tp.Size = item.Size;
                                tp.Mau = item.Mau;
                                tp.KhachHang = item.KhachHang;
                                tp.MaHang = item.MaHang;
                                tp.NgayNhap = item.NgayNhap;
                                tp.CreatedBy = _UserName;
                                tp.ModifiedBy = _UserName;
                                tp.CreatedDate = DateTime.Now;
                                tp.ModifiedDate = DateTime.Now;
                                await _context.AddAsync(tp);
                                await _context.SaveChangesAsync();

                                QuanLyThanhPham tp2 = new QuanLyThanhPham();
                                tp2 = _context.QuanLyThanhPham.FirstOrDefault(i => i.Id == tp.ThanhPhamId);
                                tp2.SoLuong += tp.SoLuong;
                                _context.Update(tp2);
                                await _context.SaveChangesAsync();
                            }
                        }

                        var _AlertMessage = "Đã cập nhật lại thông tin";
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _NhapThanhPham = vm;
                        _NhapThanhPham.CreatedDate = DateTime.Now;
                        _NhapThanhPham.ModifiedDate = DateTime.Now;
                        _NhapThanhPham.CreatedBy = _UserName;
                        _NhapThanhPham.ModifiedBy = _UserName;
                        _context.Add(_NhapThanhPham);
                        await _context.SaveChangesAsync();

                        foreach (var item in vm.ThanhPhamNhapKhoList)
                        {
                            ThanhPhamNhapKho tp = new ThanhPhamNhapKho();
                            tp.Id = item.Id;
                            tp.NhapKhoThanhPhamId = _NhapThanhPham.Id;
                            tp.SoLuong = item.SoLuong;
                            tp.DonGia = item.DonGia;
                            tp.Cancelled = item.Cancelled;
                            tp.ThanhPhamId = item.ThanhPhamId;
                            tp.PO = item.PO;
                            tp.Size = item.Size;
                            tp.Mau = item.Mau;
                            tp.KhachHang = item.KhachHang;
                            tp.MaHang = item.MaHang;
                            tp.NgayNhap = item.NgayNhap;
                            tp.CreatedBy = _UserName;
                            tp.ModifiedBy = _UserName;
                            tp.CreatedDate = DateTime.Now;
                            tp.ModifiedDate = DateTime.Now;
                            await _context.AddAsync(tp);
                            await _context.SaveChangesAsync();

                            QuanLyThanhPham tp2 = new QuanLyThanhPham();
                            tp2 = _context.QuanLyThanhPham.FirstOrDefault(i => i.Id == tp.ThanhPhamId);
                            tp2.SoLuong += tp.SoLuong;
                            _context.Update(tp2);
                            await _context.SaveChangesAsync();
                        }
                        _JsonResultViewModel.ModelObject = vm;
                        _JsonResultViewModel.AlertMessage = "Đã thêm mới thành công";
                        _JsonResultViewModel.IsSuccess = true;
                        return new JsonResult(_JsonResultViewModel);

                    }
                }
                string messages = string.Join("; ", ModelState.Values
                                         .SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage));
                _JsonResultViewModel.ModelObject = null;
                _JsonResultViewModel.AlertMessage = messages;
                _JsonResultViewModel.IsSuccess = false;
                return new JsonResult(_JsonResultViewModel);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                JsonResultViewModel _JsonResultViewModel = new();
                _JsonResultViewModel.ModelObject = null;
                _JsonResultViewModel.AlertMessage = "Có lỗi khi thêm dữ liệu vào cơ sở dữ liệu";
                _JsonResultViewModel.IsSuccess = false;
                return new JsonResult(_JsonResultViewModel);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            JsonResultViewModel _JsonResultViewModel = new();
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapThanhPham.NhapThanhPham.delete"))
            {
                try
                {
                    var _NhapThanhPham = await _context.NhapThanhPham.FindAsync(id);
                    _NhapThanhPham.ModifiedDate = DateTime.Now;
                    _NhapThanhPham.ModifiedBy = HttpContext.User.Identity.Name;
                    _NhapThanhPham.Cancelled = true;

                    _context.Update(_NhapThanhPham);
                    await _context.SaveChangesAsync();

                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _NhapThanhPham;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Xóa thất bại";
                    _JsonResultViewModel.IsSuccess = false;
                    _JsonResultViewModel.ModelObject = null;
                    return new JsonResult(_JsonResultViewModel);
                }
            }
            else if (_iCommon.CheckPermissionInProfile(user, "NhapThanhPham.NhapThanhPham.delete_owner"))
            {
                try
                {
                    var _NhapThanhPham = await _context.NhapThanhPham.FindAsync(id);
                    _NhapThanhPham.ModifiedDate = DateTime.Now;
                    _NhapThanhPham.ModifiedBy = HttpContext.User.Identity.Name;
                    _NhapThanhPham.Cancelled = true;

                    _context.Update(_NhapThanhPham);
                    await _context.SaveChangesAsync();

                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _NhapThanhPham;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Xóa thất bại";
                    _JsonResultViewModel.IsSuccess = false;
                    _JsonResultViewModel.ModelObject = null;
                    return new JsonResult(_JsonResultViewModel);
                }
            }


            _JsonResultViewModel.AlertMessage = "Bạn không có quyền xóa";
            _JsonResultViewModel.IsSuccess = true;
            _JsonResultViewModel.ModelObject = null;

            return new JsonResult(_JsonResultViewModel);

        }

        private bool IsExists(long id)
        {
            return _context.NhapThanhPham.Any(e => e.Id == id);
        }
        [HttpGet]
        public IActionResult GetAllMANhapThanhPhamByMa(string ma)
        {
            var rs = (from _ma in _context.NhapThanhPham
                      where _ma.Cancelled == false && ma.Contains(_ma.Ma)
                      select new
                      {
                          Ma = _ma.Ma
                      }
                          ).ToList();
            return Ok(rs);

        }
        public List<DtaPrint4> LoadDanhNguyenLieuNhapKhoByNhapKhoId2(int NhapKhoLyThuyetId)
        {
            return (from tblObj in _context.ThanhPhamNhapKho.Where(x => x.NhapKhoThanhPhamId == NhapKhoLyThuyetId && x.Cancelled == false).OrderBy(x => x.Id)
                    join tbl in _context.NhapThanhPham on tblObj.NhapKhoThanhPhamId equals tbl.Id
                    select new DtaPrint4
                    {
                        NgayNhap = tblObj.NgayNhap,
                        KhachHang = tblObj.KhachHang,
                        MaHang = tblObj.MaHang,
                        TenThanhPham = _context.QuanLyThanhPham.FirstOrDefault(t => t.Id == tblObj.ThanhPhamId).TenThanhPham ?? "",
                        PO = tblObj.PO,
                        Mau = tblObj.Mau,
                        Size = tblObj.Size,
                        SoLuong = tblObj.SoLuong,
                        DonGia = tblObj.DonGia,
                        DonVi = tbl.DonViTienTe,
                        MaNhap = tbl.Ma,
                        NgayNhapMain = tbl.NgayNhap,
                        ThanhTien = (tblObj.SoLuong * tblObj.DonGia) + tbl.DonViTienTe
                    }
            ).ToList();
        }

        public IActionResult Print(int id)
        {
            ViewBag.Load = LoadDanhNguyenLieuNhapKhoByNhapKhoId2(id);
            return PartialView("_Print");
        }
    }
    public class DtaPrint4
    {
        public DateTime NgayNhapMain { get; set; }
        public DateTime NgayNhap { get; set; }
        public string KhachHang { get; set; }
        public string DonVi { get; set; }
        public string MaNhap { get; set; }
        public string MaHang { get; set; }
        public string TenThanhPham { get; set; }
        public string PO { get; set; }
        public string Mau { get; set; }
        public string ThanhTien { get; set; }
        public string Size { get; set; }
        public int DonGia { get; set; }
        public int SoLuong { get; set; }

    }
}
