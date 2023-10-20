using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.NhapKhoLyThuyetViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
using AMS.Models.NguyenLieuNhapKhoLyThuyetViewModel;
using AMS.Models.NguyenLieuNhapKhoCRUDViewModel;
using Windows.Storage.Pickers.Provider;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class NhapKhoLyThuyetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;

        public NhapKhoLyThuyetController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name ?? string.Empty;
        }


        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet.NhapKhoLyThuyet.list"))
            {
                return View();
            }
            else if (_iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet.NhapKhoLyThuyet.list_owner"))
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
                    || obj.Status.ToString().ToLower().Contains(searchValue)
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
        private IQueryable<NhapKhoLyThuyetCRUDViewModel> GetGridItem()
        {
            if (_iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet.NhapKhoLyThuyet.list_owner"))
            {
                try
                {
                    return (from _NhapKhoLyThuyet in _context.NhapKhoLyThuyet
                            where _NhapKhoLyThuyet.Cancelled == false && _NhapKhoLyThuyet.CreatedBy.Contains(user)

                            select new NhapKhoLyThuyetCRUDViewModel
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

                            }).OrderByDescending(x => x.Id);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            try
            {
                return (from _NhapKhoLyThuyet in _context.NhapKhoLyThuyet
                        where _NhapKhoLyThuyet.Cancelled == false

                        select new NhapKhoLyThuyetCRUDViewModel
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
        public IActionResult GetDueNhapKho()
        {
            var today = DateTime.Today;
            var maxDueDate = today.AddDays(-2);

            var data = (from _NhapKhoLyThuyet in _context.NhapKhoLyThuyet
                        where _NhapKhoLyThuyet.Cancelled == false
                        //&& _NhapKhoLyThuyet.NgayNhap.Date <= today &&
                        //_NhapKhoLyThuyet.NgayNhap.Date > maxDueDate
                        select new NhapKhoLyThuyetCRUDViewModel
                        {
                            Id = _NhapKhoLyThuyet.Id,
                            NgayNhap = _NhapKhoLyThuyet.NgayNhap,
                            Status = _NhapKhoLyThuyet.Status,
                            DonViTienTe = _NhapKhoLyThuyet.DonViTienTe,
                            CreatedDate = _NhapKhoLyThuyet.CreatedDate,
                            ModifiedDate = _NhapKhoLyThuyet.ModifiedDate,
                            CreatedBy = _NhapKhoLyThuyet.CreatedBy,
                            ModifiedBy = _NhapKhoLyThuyet.ModifiedBy,
                            IsDue = _NhapKhoLyThuyet.NgayNhap.Date <= today
                        })
                        .OrderByDescending(x => x.NgayNhap)
                        .ToList();

            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();
            NhapKhoLyThuyetCRUDViewModel vm = await _context.NhapKhoLyThuyet.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            NhapKhoLyThuyetCRUDViewModel vm = new NhapKhoLyThuyetCRUDViewModel();

            if (id > 0)
            {
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet.NhapKhoLyThuyet.edit"))
                {
                    vm = await _context.NhapKhoLyThuyet.Where(x => x.Id == id).SingleOrDefaultAsync();
                    ViewBag.LoadNguyenLieuNhapKho = JsonConvert.SerializeObject(LoadDanhNguyenLieuNhapKhoByNhapKhoLyThuyetId(id));
                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet.NhapKhoLyThuyet.edit_owner"))
                {
                    vm = await _context.NhapKhoLyThuyet.Where(x => x.Id == id).SingleOrDefaultAsync();
                    ViewBag.LoadNguyenLieuNhapKho = JsonConvert.SerializeObject(LoadDanhNguyenLieuNhapKhoByNhapKhoLyThuyetId(id));
                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                ViewBag.LoadNguyenLieuNhapKho = JsonConvert.SerializeObject(Array.Empty<NguyenLieuNhapKhoLyThuyetCRUDViewModel>());

                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet.NhapKhoLyThuyet.create"))
                {
                    vm.Status = false;
                    return PartialView("_AddEdit", vm);
                }
            }

            return View("_AccessDeniedAddEdit");

        }

        public IQueryable<NguyenLieuNhapKhoLyThuyetCRUDViewModel> LoadDanhNguyenLieuNhapKhoByNhapKhoLyThuyetId(int NhapKhoId)
        {
            return (from tblObj in _context.NguyenLieuNhapKhoLyThuyet.Where(x => x.NhapKhoLyThuyetId == NhapKhoId && x.Cancelled == false).OrderBy(x => x.Id)
                    select new NguyenLieuNhapKhoLyThuyetCRUDViewModel
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
        public async Task<JsonResult> AddEdit(NhapKhoLyThuyetCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    NhapKhoLyThuyet _NhapKhoLyThuyet = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _NhapKhoLyThuyet = await _context.NhapKhoLyThuyet.FindAsync(vm.Id);


                        vm.CreatedDate = _NhapKhoLyThuyet.CreatedDate;
                        vm.CreatedBy = _NhapKhoLyThuyet.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_NhapKhoLyThuyet).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();
                        var _AlertMessage = "Dữ liệu đã cập nhật thành công";

                        foreach (var item in vm.NguyenLieuNhapKhoLyThuyetList)
                        {
                            NguyenLieuNhapKhoLyThuyet _nl = new NguyenLieuNhapKhoLyThuyet();

                            if (item.Id > 0)
                            {
                                _nl = _context.NguyenLieuNhapKhoLyThuyet.FirstOrDefault(x => x.Id == item.Id);
                                vm.ModifiedBy = _UserName;
                                vm.ModifiedDate = DateTime.Now;
                                _context.Entry(_nl).CurrentValues.SetValues(vm);
                            }
                            else
                            {
                                _nl = item;
                                _nl.NhapKhoLyThuyetId = _NhapKhoLyThuyet.Id;
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
                        _NhapKhoLyThuyet = vm;
                        _NhapKhoLyThuyet.CreatedDate = DateTime.Now;
                        _NhapKhoLyThuyet.ModifiedDate = DateTime.Now;
                        _NhapKhoLyThuyet.CreatedBy = _UserName;
                        _NhapKhoLyThuyet.ModifiedBy = _UserName;
                        _context.Add(_NhapKhoLyThuyet);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "Tạo mới phiếu nhập kho lý thuyết thành công";

                        foreach (var item in vm.NguyenLieuNhapKhoLyThuyetList)
                        {
                            NguyenLieuNhapKhoLyThuyet _nl = new NguyenLieuNhapKhoLyThuyet();
                            _nl = item;
                            _nl.CreatedBy = _UserName;
                            _nl.CreatedDate = DateTime.Now;
                            _nl.ModifiedBy = _UserName;
                            _nl.ModifiedDate = DateTime.Now;
                            _nl.NhapKhoLyThuyetId = _NhapKhoLyThuyet.Id;
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

            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet.NhapKhoLyThuyet.delete"))
            {
                try
                {

                    var _NhapKhoLyThuyet = await _context.NhapKhoLyThuyet.FindAsync(id);
                    _NhapKhoLyThuyet.ModifiedDate = DateTime.Now;
                    _NhapKhoLyThuyet.ModifiedBy = HttpContext.User.Identity.Name;
                    _NhapKhoLyThuyet.Cancelled = true;
                    _context.Update(_NhapKhoLyThuyet);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _NhapKhoLyThuyet;
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
            else if (_iCommon.CheckPermissionInProfile(user, "NhapKhoLyThuyet.NhapKhoLyThuyet.delete_owner"))
            {
                try
                {
                    var _NhapKhoLyThuyet = await _context.NhapKhoLyThuyet.FindAsync(id);
                    _NhapKhoLyThuyet.ModifiedDate = DateTime.Now;
                    _NhapKhoLyThuyet.ModifiedBy = HttpContext.User.Identity.Name;
                    _NhapKhoLyThuyet.Cancelled = true;
                    _context.Update(_NhapKhoLyThuyet);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _NhapKhoLyThuyet;
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
            return _context.NhapKhoLyThuyet.Any(e => e.Id == id);
        }

        [HttpGet]
        public IActionResult GetListNhapKhoLyThuyetKhiChuaNhap()
        {
            var a = (from _nk in _context.NhapKhoLyThuyet
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
        public IActionResult GetListNguyenLieuNhapKhoLyThuyetKhiChuaNhapByID(int nhakholythuyet)
        {
            var a = (from _nl in _context.NguyenLieuNhapKhoLyThuyet
                     where _nl.NhapKhoLyThuyetId == nhakholythuyet
                     select new NguyenLieuNhapKhoLyThuyetCRUDViewModel
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
                var a = _context.NhapKhoLyThuyet.FirstOrDefault(x => x.Id == id);
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
            var result = (from _ma in _context.NhapKhoLyThuyet
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
            return (from tblObj in _context.NguyenLieuNhapKhoLyThuyet.Where(x => x.NhapKhoLyThuyetId == NhapKhoLyThuyetId && x.Cancelled == false).OrderBy(x => x.Id)
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
    public class DtaPrint2
    {
        public string MaHaiQuan { get; set; }
        public string MaNhaCungCap { get; set; }
        public string NhaCungCap { get; set; }
        public string TenNguyenPhuLieu { get; set; }
        public int SoLuong { get; set; }
        public int DonGia { get; set; }
        public string TenNPL { get; set; }
        public string SoToKhai { get; set; }
    }
}
