using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.NhomKhachHangViewModel;
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
    public class NhomKhachHangController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;

        public NhomKhachHangController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name;
        }

        //[Authorize(Roles = AMS.Pages.MainMenu.NhomKhachHang.RoleName)]
        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhomKhachHang.NhomKhachHang.list"))
            {
                return View();
            }
            else if (_iCommon.CheckPermissionInProfile(user, "NhomKhachHang.NhomKhachHang.list_owner"))
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
                    || obj.TenNhomKhachHang.ToLower().Contains(searchValue)
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
        private IQueryable<NhomKhachHangCRUDViewModel> GetGridItem()
        {
            if (_iCommon.CheckPermissionInProfile(user, "NhomKhachHang.NhomKhachHang.list_owner"))
            {
                try
                {
                    return (from _NhomKhachHang in _context.NhomKhachHang
                            where _NhomKhachHang.Cancelled == false && _NhomKhachHang.CreatedBy.Contains(user)

                            select new NhomKhachHangCRUDViewModel
                            {
                                Id = _NhomKhachHang.Id,
                                TenNhomKhachHang = _NhomKhachHang.TenNhomKhachHang,
                                GhiChu = _NhomKhachHang.GhiChu,
                                CreatedDate = _NhomKhachHang.CreatedDate,
                                ModifiedDate = _NhomKhachHang.ModifiedDate,
                                CreatedBy = _NhomKhachHang.CreatedBy,
                                ModifiedBy = _NhomKhachHang.ModifiedBy,

                            }).OrderByDescending(x => x.Id);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            try
            {
                return (from _NhomKhachHang in _context.NhomKhachHang
                        where _NhomKhachHang.Cancelled == false

                        select new NhomKhachHangCRUDViewModel
                        {
                            Id = _NhomKhachHang.Id,
                            TenNhomKhachHang = _NhomKhachHang.TenNhomKhachHang,
                            GhiChu = _NhomKhachHang.GhiChu,
                            CreatedDate = _NhomKhachHang.CreatedDate,
                            ModifiedDate = _NhomKhachHang.ModifiedDate,
                            CreatedBy = _NhomKhachHang.CreatedBy,
                            ModifiedBy = _NhomKhachHang.ModifiedBy,

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
            NhomKhachHangCRUDViewModel vm = await _context.NhomKhachHang.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            NhomKhachHangCRUDViewModel vm = new NhomKhachHangCRUDViewModel();
            if (id > 0)
            {
                vm = await _context.NhomKhachHang.Where(x => x.Id == id).SingleOrDefaultAsync();
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhomKhachHang.NhomKhachHang.edit"))
                {

                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "NhomKhachHang.NhomKhachHang.edit_owner"))
                {

                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhomKhachHang.NhomKhachHang.create"))
                {

                    return PartialView("_AddEdit", vm);
                }
            }


            return PartialView("_AddEdit", vm);
        }
        //[INSERT_LOAD]

        [HttpPost]
        public async Task<JsonResult> AddEdit(NhomKhachHangCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    NhomKhachHang _NhomKhachHang = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _NhomKhachHang = await _context.NhomKhachHang.FindAsync(vm.Id);


                        vm.CreatedDate = _NhomKhachHang.CreatedDate;
                        vm.CreatedBy = _NhomKhachHang.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_NhomKhachHang).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = vm.TenNhomKhachHang+" đã cập nhật thành công";
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _NhomKhachHang = vm;


                        _NhomKhachHang.CreatedDate = DateTime.Now;
                        _NhomKhachHang.ModifiedDate = DateTime.Now;
                        _NhomKhachHang.CreatedBy = _UserName;
                        _NhomKhachHang.ModifiedBy = _UserName;
                        _context.Add(_NhomKhachHang);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = vm.TenNhomKhachHang + " đã tạo thành công";
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

            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhomKhachHang.NhomKhachHang.delete"))
            {
                try
                {
                    var _NhomKhachHang = await _context.NhomKhachHang.FindAsync(id);
                    _NhomKhachHang.ModifiedDate = DateTime.Now;
                    _NhomKhachHang.ModifiedBy = user;
                    _NhomKhachHang.Cancelled = true;

                    _context.Update(_NhomKhachHang);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _NhomKhachHang;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else if (_iCommon.CheckPermissionInProfile(user, "NhomKhachHang.NhomKhachHang.delete_owner"))
            {
                try
                {
                    var _NhomKhachHang = await _context.NhomKhachHang.FindAsync(id);
                    _NhomKhachHang.ModifiedDate = DateTime.Now;
                    _NhomKhachHang.ModifiedBy = user;
                    _NhomKhachHang.Cancelled = true;

                    _context.Update(_NhomKhachHang);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _NhomKhachHang;
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
            return _context.NhomKhachHang.Any(e => e.Id == id);
        }
    }
}
