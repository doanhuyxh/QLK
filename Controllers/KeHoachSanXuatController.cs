using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.KeHoachSanXuatViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
using AMS.Models.XayDungCongThucViewModel;
using AMS.Models.ListNguyenLieuInCongThucViewModel;
using AMS.Models.NguyenLieuKeHoachCRUDViewModel;
using Nest;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class KeHoachSanXuatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;

        public KeHoachSanXuatController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name ?? string.Empty;
        }


        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KeHoachSanXuat.KeHoachSanXuat.list"))
            {
                return View();
            }
            else if (_iCommon.CheckPermissionInProfile(user, "KeHoachSanXuat.KeHoachSanXuat.list_owner"))
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
                    || obj.TenThanhPham.ToLower().Contains(searchValue)
                    || obj.NgayDuKienHoan.ToString().ToLower().Contains(searchValue)
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
        private IQueryable<KeHoachSanXuatCRUDViewModel> GetGridItem()
        {

            if (_iCommon.CheckPermissionInProfile(user, "KeHoachSanXuat.KeHoachSanXuat.list_owner"))
            {
                try
                {
                    return (from _KeHoachSanXuat in _context.KeHoachSanXuat
                            where _KeHoachSanXuat.Cancelled == false && _KeHoachSanXuat.CreatedBy.Contains(user)

                            select new KeHoachSanXuatCRUDViewModel
                            {
                                Id = _KeHoachSanXuat.Id,
                                TenThanhPham = _KeHoachSanXuat.TenThanhPham,
                                NgayDuKienHoan = _KeHoachSanXuat.NgayDuKienHoan,
                                CreatedDate = _KeHoachSanXuat.CreatedDate,
                                ModifiedDate = _KeHoachSanXuat.ModifiedDate,
                                CreatedBy = _KeHoachSanXuat.CreatedBy,
                                ModifiedBy = _KeHoachSanXuat.ModifiedBy,

                            }).OrderByDescending(x => x.Id);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            try
            {
                return (from _KeHoachSanXuat in _context.KeHoachSanXuat
                        where _KeHoachSanXuat.Cancelled == false

                        select new KeHoachSanXuatCRUDViewModel
                        {
                            Id = _KeHoachSanXuat.Id,
                            TenThanhPham = _KeHoachSanXuat.TenThanhPham,
                            NgayDuKienHoan = _KeHoachSanXuat.NgayDuKienHoan,
                            CreatedDate = _KeHoachSanXuat.CreatedDate,
                            ModifiedDate = _KeHoachSanXuat.ModifiedDate,
                            CreatedBy = _KeHoachSanXuat.CreatedBy,
                            ModifiedBy = _KeHoachSanXuat.ModifiedBy,

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
            KeHoachSanXuatCRUDViewModel vm = await _context.KeHoachSanXuat.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            KeHoachSanXuatCRUDViewModel vm = new KeHoachSanXuatCRUDViewModel();

            if (id > 0)
            {
                vm = await _context.KeHoachSanXuat.Where(x => x.Id == id).SingleOrDefaultAsync();
                ViewBag._LoadThanhPhamSanXuat = JsonConvert.SerializeObject(LoadNguyenLieuId(id));
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KeHoachSanXuat.KeHoachSanXuat.edit"))
                {
                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "KeHoachSanXuat.KeHoachSanXuat.edit_owner"))
                {
                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                ViewBag._LoadThanhPhamSanXuat = JsonConvert.SerializeObject(Array.Empty<NguyenLieuKeHoachCRUDViewModel>());
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KeHoachSanXuat.KeHoachSanXuat.create"))
                {
                    return PartialView("_AddEdit", vm);
                }
            }
            return View("_AccessDeniedAddEdit");

        }
        public IQueryable<NguyenLieuKeHoachCRUDViewModel> LoadNguyenLieuId(int sanxuatId)
        {

            return (from tblObj in _context.NguyenLieuKeHoach

                    .Where(x => x.KeHoachSanXuatId == sanxuatId && x.Cancelled == false)
                    .OrderBy(x => x.Id)

                    select new NguyenLieuKeHoachCRUDViewModel
                    {
                        Id = tblObj.Id,
                        NgayDuKien = tblObj.NgayDuKien,
                        SanPhamId = tblObj.SanPhamId,
                        SoMetChi = tblObj.SoMetChi,
                        SoLo = tblObj.SoLo,
                        SoLuong = tblObj.SoLuong,
                        DinhMuc = tblObj.DinhMuc,
                        DonVi = tblObj.DonVi,
                        NgayVeVIT = tblObj.NgayVeVIT,
                        NguyenLieuId = tblObj.NguyenLieuId,
                        NhuCau = tblObj.NhuCau,
                        ThucNhan = tblObj.ThucNhan,
                        CachSuDung = tblObj.CachSuDung,
                        ListSizeMau = tblObj.ListSizeMau,
                        KeHoachSanXuatId = tblObj.KeHoachSanXuatId,
                        MieuTa = tblObj.MieuTa,

                    }

                );
        }

        public ActionResult<XayDungCongThucCRUDViewModels> GetApiCongThuc()
        {
            var x = from tp in _context.XayDungCongThuc
                    join _listNguyenLieu in _context.ListNguyenLieuInCongThuc on tp.Id equals _listNguyenLieu.SanPhamId
                    join _nl in _context.NguyenLieu on _listNguyenLieu.NguyenLieuId equals _nl.Id

                    where tp.Cancelled == false
                    select new { tp, _listNguyenLieu, _nl };
            var result = new List<XayDungCongThucCRUDViewModels>();
            foreach (var item in x)
            {
                var xayDungCongThucCRUDViewModel = new XayDungCongThucCRUDViewModels
                {
                    Id = item.tp.Id,
                    Name = item.tp.Name,
                    Description = item.tp.Description,
                    MaSP = item.tp.MaSP,
                    Cancelled = false,
                    ListNguyenLieuInCongThuc = new List<ListNguyenLieuInCongThucViewModel>
                    {

                        new ListNguyenLieuInCongThucViewModel
                        {
                            Id = item._listNguyenLieu.Id,
                            MieuTa = item._listNguyenLieu.MieuTa,
                            NguyenLieuId = item._listNguyenLieu.NguyenLieuId,
                            SanPhamId = item.tp.Id,
                            DonVi = item._listNguyenLieu.DonVi,
                            SoLuong = item._listNguyenLieu.SoLuong,
                            DinhMuc = item._listNguyenLieu.DinhMuc,
                            NhuCau = item._listNguyenLieu.NhuCau,
                            SoLo = item._listNguyenLieu.SoLo,
                            NgayVeVIT = item._listNguyenLieu.NgayVeVIT,
                            ThucNhan = item._listNguyenLieu.ThucNhan,
                            ListSizeMau = item._listNguyenLieu.ListSizeMau


                        }
                    }

                };
                result.Add(xayDungCongThucCRUDViewModel);
            }


            return Ok(result);
        }

        [HttpPost]
        public async Task<JsonResult> AddEdit(KeHoachSanXuatCRUDViewModel vm)
        {
            try
            {
                JsonResultViewModel _JsonResultViewModel = new();

                if (ModelState.IsValid)
                {
                    KeHoachSanXuat _KeHoachSanXuat = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _KeHoachSanXuat = await _context.KeHoachSanXuat.FindAsync(vm.Id);
                        vm.CreatedDate = _KeHoachSanXuat.CreatedDate;
                        vm.CreatedBy = _KeHoachSanXuat.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_KeHoachSanXuat).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();
                        foreach (var item in vm.ListNguyenLieuKeHoach)
                        {
                            NguyenLieuKeHoach nl = new NguyenLieuKeHoach();
                            if (item.Id > 0)
                            {
                                nl = await _context.NguyenLieuKeHoach.FindAsync(item.Id);
                                item.ModifiedBy = _UserName;
                                item.ModifiedDate = DateTime.Now;
                                _context.Entry(nl).CurrentValues.SetValues(item);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                nl = item;
                                nl.KeHoachSanXuatId = _KeHoachSanXuat.Id;
                                nl.NgayDuKien = vm.NgayDuKienHoan;
                                nl.CreatedDate = DateTime.Now;
                                nl.ModifiedDate = DateTime.Now;
                                nl.CreatedBy = _UserName;
                                nl.ModifiedBy = _UserName;
                                _context.Add(nl);
                                await _context.SaveChangesAsync();
                            }
                        }

                        _JsonResultViewModel.ModelObject = vm;
                        _JsonResultViewModel.AlertMessage = " Đã cập nhật thành công";
                        _JsonResultViewModel.IsSuccess = true;
                        return new JsonResult(_JsonResultViewModel);
                    }
                    else
                    {
                        _KeHoachSanXuat = vm;
                        _KeHoachSanXuat.CreatedDate = DateTime.Now;
                        _KeHoachSanXuat.ModifiedDate = DateTime.Now;
                        _KeHoachSanXuat.CreatedBy = _UserName;
                        _KeHoachSanXuat.ModifiedBy = _UserName;
                        _context.Add(_KeHoachSanXuat);
                        await _context.SaveChangesAsync();
                        foreach (var item in vm.ListNguyenLieuKeHoach)
                        {
                            NguyenLieuKeHoach nl = new NguyenLieuKeHoach();
                            nl = item;
                            nl.KeHoachSanXuatId = _KeHoachSanXuat.Id;
                            nl.NgayDuKien = vm.NgayDuKienHoan;
                            nl.CreatedDate = DateTime.Now;
                            nl.ModifiedDate = DateTime.Now;
                            nl.CreatedBy = _UserName;
                            nl.ModifiedBy = _UserName;
                            _context.Add(nl);
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

            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KeHoachSanXuat.KeHoachSanXuat.delete"))
            {
                try
                {
                    var _KeHoachSanXuat = await _context.KeHoachSanXuat.FindAsync(id);
                    _KeHoachSanXuat.ModifiedDate = DateTime.Now;
                    _KeHoachSanXuat.ModifiedBy = HttpContext.User.Identity.Name;
                    _KeHoachSanXuat.Cancelled = true;

                    _context.Update(_KeHoachSanXuat);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xoá thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _KeHoachSanXuat;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else if (_iCommon.CheckPermissionInProfile(user, "KeHoachSanXuat.KeHoachSanXuat.delete_owner"))
            {
                try
                {
                    var _KeHoachSanXuat = await _context.KeHoachSanXuat.FindAsync(id);
                    _KeHoachSanXuat.ModifiedDate = DateTime.Now;
                    _KeHoachSanXuat.ModifiedBy = HttpContext.User.Identity.Name;
                    _KeHoachSanXuat.Cancelled = true;

                    _context.Update(_KeHoachSanXuat);
                    await _context.SaveChangesAsync();

                    _JsonResultViewModel.AlertMessage = "Đã xoá thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _KeHoachSanXuat;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Có lỗi khi xoá";
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
            return _context.KeHoachSanXuat.Any(e => e.Id == id);
        }
        public IActionResult GetDanhSachKeHoach()
        {
            var ds = (from _kh in _context.KeHoachSanXuat
                      where _kh.Cancelled == false
                      select new
                      {
                          Id = _kh.Id,
                          TenKeHoach = _kh.TenThanhPham
                      }).ToList();
            return Ok(ds);
        }
        public IActionResult GetNguyenLieuKeHoachByKeHoachId(int id)
        {
            var rs = (
                from _nl in _context.NguyenLieuKeHoach
                where _nl.KeHoachSanXuatId == id && _nl.Cancelled == false
                select new
                {
                    Id = _nl.Id,
                    MieuTa = _nl.MieuTa,
                    NguyenLieuId = _nl.NguyenLieuId,
                    KeHoachSanXuatId = _nl.KeHoachSanXuatId,
                    SanPhamId = _nl.SanPhamId,
                    SoMetChi = _nl.SoMetChi,
                    CachSuDung = _nl.CachSuDung,
                    DonVi = _nl.DonVi,
                    SoLuong = _nl.SoLuong,
                    DinhMuc = _nl.DinhMuc,
                    NhuCau = _nl.NhuCau,
                    SoLo = _nl.SoLo,
                    NgayVeVIT = _nl.NgayVeVIT,
                    ThucNhan = _nl.ThucNhan,
                    ListSizeMau = _nl.ListSizeMau,

                }
                ).ToList();

            return Ok(rs);
        }
    }
}
