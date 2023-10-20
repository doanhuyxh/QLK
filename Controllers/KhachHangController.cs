using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.KhachHangViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class KhachHangController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;

        public KhachHangController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KhachHang.KhachHang.list"))
            {
                return View();
            }
            else if (_iCommon.CheckPermissionInProfile(user, "KhachHang.KhachHang.list_owner"))
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
                    || obj.HoVaTen.ToLower().Contains(searchValue)
                    || obj.SoDienThoai.ToLower().Contains(searchValue)
                    || obj.DiaChi.ToLower().Contains(searchValue)
                    || obj.GhiChu.ToLower().Contains(searchValue)
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
        private IQueryable<KhachHangCRUDViewModel> GetGridItem()
        {
            if (_iCommon.CheckPermissionInProfile(user, "KhachHang.KhachHang.list_owner"))
            {
                try
                {
                    return (from _KhachHang in _context.KhachHang
                            where _KhachHang.Cancelled == false && _KhachHang.CreatedBy.Contains(user)

                            join _NhomKhachHang in _context.NhomKhachHang on _KhachHang.NhomKhachHangId equals _NhomKhachHang.Id
                            into listNhomKhachHang
                            from _NhomKhachHang in listNhomKhachHang.DefaultIfEmpty()
                            select new KhachHangCRUDViewModel
                            {
                                Id = _KhachHang.Id,
                                HoVaTen = _KhachHang.HoVaTen,
                                SoDienThoai = _KhachHang.SoDienThoai,
                                DiaChi = _KhachHang.DiaChi,
                                GhiChu = _KhachHang.GhiChu,
                                NhomKhachHangDisplay = _NhomKhachHang.TenNhomKhachHang,
                                CreatedDate = _KhachHang.CreatedDate,
                                ModifiedDate = _KhachHang.ModifiedDate,
                                CreatedBy = _KhachHang.CreatedBy,
                                ModifiedBy = _KhachHang.ModifiedBy,

                            }).OrderByDescending(x => x.Id);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            try
            {
                return (from _KhachHang in _context.KhachHang
                        where _KhachHang.Cancelled == false

                        join _NhomKhachHang in _context.NhomKhachHang on _KhachHang.NhomKhachHangId equals _NhomKhachHang.Id
                        into listNhomKhachHang
                        from _NhomKhachHang in listNhomKhachHang.DefaultIfEmpty()

                        select new KhachHangCRUDViewModel
                        {
                            Id = _KhachHang.Id,
                            HoVaTen = _KhachHang.HoVaTen,
                            SoDienThoai = _KhachHang.SoDienThoai,
                            DiaChi = _KhachHang.DiaChi,
                            GhiChu = _KhachHang.GhiChu,
                            NhomKhachHangDisplay = _NhomKhachHang.TenNhomKhachHang,
                            CreatedDate = _KhachHang.CreatedDate,
                            ModifiedDate = _KhachHang.ModifiedDate,
                            CreatedBy = _KhachHang.CreatedBy,
                            ModifiedBy = _KhachHang.ModifiedBy,

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
            KhachHangCRUDViewModel vm = await _context.KhachHang.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            KhachHangCRUDViewModel vm = new KhachHangCRUDViewModel();
            if (id > 0)
            {
                vm = await _context.KhachHang.Where(x => x.Id == id).SingleOrDefaultAsync();

                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KhachHang.KhachHang.edit"))
                {
                    ViewBag.LoaddNhomKhachHang = new SelectList(LoadDanhLoaiKhachHang(), "Id", "Name");
                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "KhachHang.KhachHang.edit_owner"))
                {
                    ViewBag.LoaddNhomKhachHang = new SelectList(LoadDanhLoaiKhachHang(), "Id", "Name");
                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KhachHang.KhachHang.create"))
                {
                    ViewBag.LoaddNhomKhachHang = new SelectList(LoadDanhLoaiKhachHang(), "Id", "Name");
                    return PartialView("_AddEdit", vm);
                }
            }

            return View("_AccessDeniedAddEdit");
        }

        public IQueryable<ItemDropdownListViewModel> LoadDanhLoaiKhachHang()
        {
            return (from tblObj in _context.NhomKhachHang.Where(x => x.Cancelled == false).OrderBy(x => x.Id)
                    select new ItemDropdownListViewModel
                    {
                        Id = tblObj.Id,
                        Name = tblObj.TenNhomKhachHang
                    }
            );
        }


        [HttpPost]
        public async Task<JsonResult> AddEdit(KhachHangCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    KhachHang _KhachHang = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _KhachHang = await _context.KhachHang.FindAsync(vm.Id);


                        vm.CreatedDate = _KhachHang.CreatedDate;
                        vm.CreatedBy = _KhachHang.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_KhachHang).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "Khách hàng: " + vm.HoVaTen + " đã cập nhật thành công";
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _KhachHang = vm;


                        _KhachHang.CreatedDate = DateTime.Now;
                        _KhachHang.ModifiedDate = DateTime.Now;
                        _KhachHang.CreatedBy = _UserName;
                        _KhachHang.ModifiedBy = _UserName;
                        _context.Add(_KhachHang);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "Khách hàng: " + vm.HoVaTen + " đã tạo thành công";
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

            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KhachHang.KhachHang.delete"))
            {
                try
                {
                    var _KhachHang = await _context.KhachHang.FindAsync(id);
                    _KhachHang.ModifiedDate = DateTime.Now;
                    _KhachHang.ModifiedBy = HttpContext.User.Identity.Name;
                    _KhachHang.Cancelled = true;

                    _context.Update(_KhachHang);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _KhachHang;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else if (_iCommon.CheckPermissionInProfile(user, "KhachHang.KhachHang.delete_owner"))
            {
                try
                {
                    var _KhachHang = await _context.KhachHang.FindAsync(id);
                    _KhachHang.ModifiedDate = DateTime.Now;
                    _KhachHang.ModifiedBy = HttpContext.User.Identity.Name;
                    _KhachHang.Cancelled = true;

                    _context.Update(_KhachHang);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _KhachHang;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            _JsonResultViewModel.AlertMessage = "Bạn không có quyền xóa";
            _JsonResultViewModel.IsSuccess = true;
            _JsonResultViewModel.ModelObject = null;

            return new JsonResult(_JsonResultViewModel);

        }

        private bool IsExists(long id)
        {
            return _context.KhachHang.Any(e => e.Id == id);
        }
    }
}
