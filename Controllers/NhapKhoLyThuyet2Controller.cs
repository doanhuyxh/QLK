using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.NhapKhoLyThuyet2ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
using AMS.Models.NguyenLieuNhapKhoLyThuyetViewModel;
using AMS.Models.NhapKhoLyThuyetViewModel;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class NhapKhoLyThuyet2Controller : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;

        public NhapKhoLyThuyet2Controller(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name ?? string.Empty;
        }


        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet2.NhapKhoLyThuyet2.list"))
            {
                return View();
            }
            else if (_iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet2.NhapKhoLyThuyet2.list_owner"))
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
                    || obj.MaPhieu.ToLower().Contains(searchValue)
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
        private IQueryable<NhapKhoLyThuyet2CRUDViewModel> GetGridItem()
        {
            if (_iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet2.NhapKhoLyThuyet2.list_owner"))
            {
                try
                {
                    return (from _NhapKhoLyThuyet in _context.NhapKhoLyThuyet2
                            where _NhapKhoLyThuyet.Cancelled == false && _NhapKhoLyThuyet.CreatedBy.Contains(user)

                            select new NhapKhoLyThuyet2CRUDViewModel
                            {
                                Id = _NhapKhoLyThuyet.Id,
                                MaPhieu = _NhapKhoLyThuyet.MaPhieu,
                                MaSoLo = _NhapKhoLyThuyet.MaSoLo,
                                TenKhachHang = _NhapKhoLyThuyet.TenKhachHang,
                                NgayNhap = _NhapKhoLyThuyet.NgayNhap,
                                DuKienNgayVe = _NhapKhoLyThuyet.DuKienNgayVe,
                                Status = _NhapKhoLyThuyet.Status,
                                DonViTienTe = _NhapKhoLyThuyet.DonViTienTe,
                                CreatedDate = _NhapKhoLyThuyet.CreatedDate,
                                ModifiedDate = _NhapKhoLyThuyet.ModifiedDate,
                                CreatedBy = _NhapKhoLyThuyet.CreatedBy,
                                ModifiedBy = _NhapKhoLyThuyet.ModifiedBy,
                                SoToKhai = _NhapKhoLyThuyet.SoToKhai,

                            }).OrderByDescending(x => x.Id);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            try
            {
                return (from _NhapKhoLyThuyet in _context.NhapKhoLyThuyet2
                        where _NhapKhoLyThuyet.Cancelled == false

                        select new NhapKhoLyThuyet2CRUDViewModel
                        {
                            Id = _NhapKhoLyThuyet.Id,
                            NgayNhap = _NhapKhoLyThuyet.NgayNhap,
                            MaPhieu = _NhapKhoLyThuyet.MaPhieu,
                            MaSoLo = _NhapKhoLyThuyet.MaSoLo,
                            TenKhachHang = _NhapKhoLyThuyet.TenKhachHang,
                            Status = _NhapKhoLyThuyet.Status,
                            DonViTienTe = _NhapKhoLyThuyet.DonViTienTe,
                            DuKienNgayVe = _NhapKhoLyThuyet.DuKienNgayVe,
                            CreatedDate = _NhapKhoLyThuyet.CreatedDate,
                            ModifiedDate = _NhapKhoLyThuyet.ModifiedDate,
                            CreatedBy = _NhapKhoLyThuyet.CreatedBy,
                            SoToKhai = _NhapKhoLyThuyet.SoToKhai,
                            ModifiedBy = _NhapKhoLyThuyet.ModifiedBy,

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
            NhapKhoLyThuyet2CRUDViewModel vm = await _context.NhapKhoLyThuyet2.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {

            NhapKhoLyThuyet2CRUDViewModel vm = new NhapKhoLyThuyet2CRUDViewModel();
            if (id > 0)
            {
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet2.NhapKhoLyThuyet2.edit"))
                {
                    vm = await _context.NhapKhoLyThuyet2.Where(x => x.Id == id).SingleOrDefaultAsync();
                    ViewBag.LoadNguyenLieuNhapKho = JsonConvert.SerializeObject(LoadDanhNguyenLieuNhapKhoByNhapKhoLyThuyet2Id(id));
                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet.NhapKhoLyThuyet.edit_owner"))
                {
                    vm = await _context.NhapKhoLyThuyet2.Where(x => x.Id == id).SingleOrDefaultAsync();
                    ViewBag.LoadNguyenLieuNhapKho = JsonConvert.SerializeObject(LoadDanhNguyenLieuNhapKhoByNhapKhoLyThuyet2Id(id));
                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                ViewBag.LoadNguyenLieuNhapKho = JsonConvert.SerializeObject(Array.Empty<NguyenLieuNhapKhoLyThuyet2CRUDViewModel>());

                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet2.NhapKhoLyThuyet2.create"))
                {
                    vm.Status = false;
                    return PartialView("_AddEdit", vm);
                }
            }

            return View("_AccessDeniedAddEdit");
        }

        public IQueryable<NguyenLieuNhapKhoLyThuyet2CRUDViewModel> LoadDanhNguyenLieuNhapKhoByNhapKhoLyThuyet2Id(int NhapKhoId)
        {
            return (from tblObj in _context.NguyenLieuNhapKhoLyThuyet2.Where(x => x.NhapKhoLyThuyetId == NhapKhoId && x.Cancelled == false).OrderBy(x => x.Id)
                    select new NguyenLieuNhapKhoLyThuyet2CRUDViewModel
                    {
                        Id = tblObj.Id,
                        NhapKhoLyThuyetId = tblObj.NhapKhoLyThuyetId,
                        MaHaiQuan = tblObj.MaHaiQuan,
                        MaNhaCungCap = tblObj.MaNhaCungCap,
                        NhaCungCap = tblObj.NhaCungCap,
                        NguyenLieuId = tblObj.NguyenLieuId,
                        SoLuongMua = tblObj.SoLuongMua,
                        DonGia = tblObj.DonGia,
                        NgayNhap = tblObj.NgayNhap,
                        ThanhPhan = tblObj.ThanhPhan,
                        DateDisplay = tblObj.NgayNhap.ToString("dd/MM/yyyy"),
                        DonViTinh = tblObj.DonViTinh,
                        GhiChu = tblObj.GhiChu,
                        Cancelled = tblObj.Cancelled,
                    }
            );
        }

        [HttpPost]
        public async Task<JsonResult> AddEdit(NhapKhoLyThuyet2CRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    NhapKhoLyThuyet2 _NhapKhoLyThuyet2 = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _NhapKhoLyThuyet2 = await _context.NhapKhoLyThuyet2.FindAsync(vm.Id);


                        vm.CreatedDate = _NhapKhoLyThuyet2.CreatedDate;
                        vm.CreatedBy = _NhapKhoLyThuyet2.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_NhapKhoLyThuyet2).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();
                        var _AlertMessage = "Dữ liệu đã cập nhật thành công";

                        foreach (var item in vm.NguyenLieuNhapKhoLyThuyetList2)
                        {
                            NguyenLieuNhapKhoLyThuyet2 _nl = new NguyenLieuNhapKhoLyThuyet2();

                            if (item.Id > 0)
                            {
                                _nl = _context.NguyenLieuNhapKhoLyThuyet2.FirstOrDefault(x => x.Id == item.Id);
                                vm.ModifiedBy = _UserName;
                                vm.ModifiedDate = DateTime.Now;
                                _context.Entry(_nl).CurrentValues.SetValues(vm);
                            }
                            else
                            {
                                _nl = item;
                                _nl.NhapKhoLyThuyetId = _NhapKhoLyThuyet2.Id;
                                _nl.CreatedBy = _UserName;
                                _nl.CreatedDate = DateTime.Now;
                                _nl.ModifiedBy = _UserName;
                                _nl.ModifiedDate = DateTime.Now;
                                _context.Add(_nl);
                                _context.SaveChanges();
                            }
                        }
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _NhapKhoLyThuyet2 = vm;
                        _NhapKhoLyThuyet2.CreatedDate = DateTime.Now;
                        _NhapKhoLyThuyet2.ModifiedDate = DateTime.Now;
                        _NhapKhoLyThuyet2.CreatedBy = _UserName;
                        _NhapKhoLyThuyet2.ModifiedBy = _UserName;
                        _context.Add(_NhapKhoLyThuyet2);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "Tạo mới phiếu nhập kho lý thuyết thành công";

                        foreach (var item in vm.NguyenLieuNhapKhoLyThuyetList2)
                        {
                            NguyenLieuNhapKhoLyThuyet2 _nl = new NguyenLieuNhapKhoLyThuyet2();
                            _nl = item;
                            _nl.CreatedBy = _UserName;
                            _nl.CreatedDate = DateTime.Now;
                            _nl.ModifiedBy = _UserName;
                            _nl.ModifiedDate = DateTime.Now;
                            _nl.NhapKhoLyThuyetId = _NhapKhoLyThuyet2.Id;
                            _context.Add(_nl);
                            _context.SaveChanges();

                        };
                        return new JsonResult(_AlertMessage);
                    }
                }
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                return new JsonResult(messages);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new JsonResult(ex.Message);
                throw ex;
            }
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            JsonResultViewModel _JsonResultViewModel = new();

            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet2.NhapKhoLyThuyet2.delete"))
            {
                try
                {

                    var _NhapKhoLyThuyet2 = await _context.NhapKhoLyThuyet2.FindAsync(id);
                    _NhapKhoLyThuyet2.ModifiedDate = DateTime.Now;
                    _NhapKhoLyThuyet2.ModifiedBy = HttpContext.User.Identity.Name;
                    _NhapKhoLyThuyet2.Cancelled = true;
                    _context.Update(_NhapKhoLyThuyet2);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _NhapKhoLyThuyet2;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Xóa thất bại";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = null;
                    return new JsonResult(_JsonResultViewModel);
                }
            }
            else if (_iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet2.NhapKhoLyThuyet2.delete_owner"))
            {
                try
                {
                    var _NhapKhoLyThuyet2 = await _context.NhapKhoLyThuyet2.FindAsync(id);
                    _NhapKhoLyThuyet2.ModifiedDate = DateTime.Now;
                    _NhapKhoLyThuyet2.ModifiedBy = HttpContext.User.Identity.Name;
                    _NhapKhoLyThuyet2.Cancelled = true;
                    _context.Update(_NhapKhoLyThuyet2);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _NhapKhoLyThuyet2;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Xóa thất bại";
                    _JsonResultViewModel.IsSuccess = true;
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
            return _context.NhapKhoLyThuyet2.Any(e => e.Id == id);
        }

        public IActionResult GetListNhapKhoLyThuyetKhiChuaNhap()
        {
            var a = (from _nk in _context.NhapKhoLyThuyet2
                     where _nk.Cancelled == false && _nk.Status == false
                     select new
                     {
                         Id = _nk.Id,
                         Name = _nk.MaPhieu,
                         MaLo = _nk.MaSoLo

                     }
                     ).ToList();

            return Ok(a);
        }
        [HttpGet]
        public IActionResult GetListNguyenLieuNhapKhoLyThuye2tKhiChuaNhapByID(int nhakholythuyet)
        {
            var a = (from _nl in _context.NguyenLieuNhapKhoLyThuyet2
                     where _nl.NhapKhoLyThuyetId == nhakholythuyet
                     select new NguyenLieuNhapKhoLyThuyet2CRUDViewModel
                     {
                         Id = _nl.Id,
                         NhapKhoLyThuyetId = _nl.NhapKhoLyThuyetId,
                         MaHaiQuan = _nl.MaHaiQuan,
                         MaNhaCungCap = _nl.MaNhaCungCap,
                         NhaCungCap = _nl.NhaCungCap,
                         NguyenLieuId = _nl.NguyenLieuId,
                         SoLuongMua = _nl.SoLuongMua,
                         DonGia = _nl.DonGia,
                         NgayNhap = _nl.NgayNhap,
                         GhiChu = _nl.GhiChu,
                         Cancelled = _nl.Cancelled,
                     }
                ).ToList();
            return Ok(a);
        }

        [HttpGet]
        public IActionResult ChuyenSangTrangThaiDaNhap(int id)
        {
            try
            {
                var a = _context.NhapKhoLyThuyet2.FirstOrDefault(x => x.Id == id);
                if (a != null)
                {
                    a.Status = true;
                    _context.Update(a);
                    _context.SaveChanges();
                    return Ok(a);
                }
                return NotFound();
            }
            catch
            {
                return BadRequest();
            }
        }

        public IActionResult GetMaPhieuByName(string name)
        {
            var result = (from _ma in _context.NhapKhoLyThuyet2
                          where _ma.Cancelled == false && _ma.MaPhieu.StartsWith(name)
                          select new
                          {
                              Ma = _ma.MaPhieu
                          }
                          ).ToList();
            return Ok(result);
        }

        public List<DtaPrint2> LoadDanhNguyenLieuNhapKhoByNhapKhoId2(int NhapKhoLyThuyetId)
        {
            return (from tblObj in _context.NguyenLieuNhapKhoLyThuyet2.Where(x => x.NhapKhoLyThuyetId == NhapKhoLyThuyetId && x.Cancelled == false).OrderBy(x => x.Id)
                    select new DtaPrint2
                    {
                        MaHaiQuan = tblObj.MaHaiQuan,
                        MaNhaCungCap = tblObj.MaNhaCungCap,
                        NhaCungCap = tblObj.NhaCungCap,
                        TenNPL = _context.NguyenLieu.FirstOrDefault(i => i.Id == tblObj.NguyenLieuId).TenNguyenLieu ?? "",
                        SoLuong = tblObj.SoLuongMua,
                        DonGia = tblObj.DonGia,
                    }
            ).ToList();
        }

        public IActionResult Print(int id)
        {
            ViewBag.Load = LoadDanhNguyenLieuNhapKhoByNhapKhoId2(id);
            return PartialView("_Print");
        }
    }
}
