using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.QuanLyThanhPhamViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
using AMS.Models.NguyenLieuKeHoachCRUDViewModel;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class QuanLyThanhPhamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;


        public QuanLyThanhPhamController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name ?? string.Empty;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "QuanLyThanhPham.QuanLyThanhPham.list"))
            {
                return View();
            }
            else if (_iCommon.CheckPermissionInProfile(user, "QuanLyThanhPham.QuanLyThanhPham.list_owner"))
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
                    || obj.GhiChu.ToLower().Contains(searchValue)
                    || obj.KhachHang.ToLower().Contains(searchValue)
                    || obj.Mau.ToLower().Contains(searchValue)
                    || obj.Size.ToLower().Contains(searchValue)
                    || obj.PO.ToLower().Contains(searchValue)
                    || obj.SoLuong.ToString().ToLower().Contains(searchValue)
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
        private IQueryable<QuanLyThanhPhamCRUDViewModel> GetGridItem()
        {
            if (_iCommon.CheckPermissionInProfile(user, "QuanLyThanhPham.QuanLyThanhPham.list_owner"))
            {
                return (from _QuanLyThanhPham in _context.QuanLyThanhPham
                        where _QuanLyThanhPham.Cancelled == false && _QuanLyThanhPham.CreatedBy.Contains(user)

                        select new QuanLyThanhPhamCRUDViewModel
                        {
                            Id = _QuanLyThanhPham.Id,
                            TenThanhPham = _QuanLyThanhPham.TenThanhPham,
                            GhiChu = _QuanLyThanhPham.GhiChu,
                            KhachHang = _QuanLyThanhPham.KhachHang,
                            Mau = _QuanLyThanhPham.Mau,
                            Size = _QuanLyThanhPham.Size,
                            PO = _QuanLyThanhPham.PO,
                            SoLuong = _QuanLyThanhPham.SoLuong,
                            CreatedDate = _QuanLyThanhPham.CreatedDate,
                            ModifiedDate = _QuanLyThanhPham.ModifiedDate,
                            CreatedBy = _QuanLyThanhPham.CreatedBy,
                            ModifiedBy = _QuanLyThanhPham.ModifiedBy,

                        }).OrderByDescending(x => x.Id);

            }

            try
            {
                return (from _QuanLyThanhPham in _context.QuanLyThanhPham
                        where _QuanLyThanhPham.Cancelled == false

                        select new QuanLyThanhPhamCRUDViewModel
                        {
                            Id = _QuanLyThanhPham.Id,
                            TenThanhPham = _QuanLyThanhPham.TenThanhPham,
                            GhiChu = _QuanLyThanhPham.GhiChu,
                            KhachHang = _QuanLyThanhPham.KhachHang,
                            Mau = _QuanLyThanhPham.Mau,
                            Size = _QuanLyThanhPham.Size,
                            PO = _QuanLyThanhPham.PO,
                            SoLuong = _QuanLyThanhPham.SoLuong,
                            CreatedDate = _QuanLyThanhPham.CreatedDate,
                            ModifiedDate = _QuanLyThanhPham.ModifiedDate,
                            CreatedBy = _QuanLyThanhPham.CreatedBy,
                            ModifiedBy = _QuanLyThanhPham.ModifiedBy,

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
            QuanLyThanhPhamCRUDViewModel vm = await _context.QuanLyThanhPham.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            QuanLyThanhPhamCRUDViewModel vm = new QuanLyThanhPhamCRUDViewModel();


            if (id > 0)
            {
                vm = vm = await _context.QuanLyThanhPham.Where(x => x.Id == id).SingleOrDefaultAsync();
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "QuanLyThanhPham.QuanLyThanhPham.edit"))
                {
                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "QuanLyThanhPham.QuanLyThanhPham.edit_owner"))
                {
                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "QuanLyThanhPham.QuanLyThanhPham.create"))
                {
                    return PartialView("_AddEdit", vm);
                }
            }
            return View("_AccessDeniedAddEdit");

        }

        [HttpPost]
        public async Task<JsonResult> AddEdit(QuanLyThanhPhamCRUDViewModel vm)
        {
            try
            {
                JsonResultViewModel _JsonResultViewModel = new();

                if (ModelState.IsValid)
                {
                    QuanLyThanhPham _QuanLyThanhPham = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _QuanLyThanhPham = await _context.QuanLyThanhPham.FindAsync(vm.Id);


                        vm.CreatedDate = _QuanLyThanhPham.CreatedDate;
                        vm.CreatedBy = _QuanLyThanhPham.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_QuanLyThanhPham).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "Thành phẩm " + vm.TenThanhPham + " cập nhật thành công";
                        _JsonResultViewModel.ModelObject = vm;
                        _JsonResultViewModel.AlertMessage = _AlertMessage;
                        _JsonResultViewModel.IsSuccess = true;
                        return new JsonResult(_JsonResultViewModel);
                    }
                    else
                    {
                        _QuanLyThanhPham = vm;


                        _QuanLyThanhPham.CreatedDate = DateTime.Now;
                        _QuanLyThanhPham.ModifiedDate = DateTime.Now;
                        _QuanLyThanhPham.CreatedBy = _UserName;
                        _QuanLyThanhPham.ModifiedBy = _UserName;
                        _context.Add(_QuanLyThanhPham);
                        await _context.SaveChangesAsync();

                        _JsonResultViewModel.ModelObject = vm;
                        _JsonResultViewModel.AlertMessage = "Thành phẩm " + vm.TenThanhPham + " đã tạo mới thành công";
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


            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "QuanLyThanhPham.QuanLyThanhPham.delete"))
            {
                try
                {
                    var _QuanLyThanhPham = await _context.QuanLyThanhPham.FindAsync(id);
                    _QuanLyThanhPham.ModifiedDate = DateTime.Now;
                    _QuanLyThanhPham.ModifiedBy = HttpContext.User.Identity.Name;
                    _QuanLyThanhPham.Cancelled = true;

                    _context.Update(_QuanLyThanhPham);
                    await _context.SaveChangesAsync();

                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _QuanLyThanhPham;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Đã xóa thất bại";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = null;
                    return new JsonResult(_JsonResultViewModel);
                }
            }
            else if (_iCommon.CheckPermissionInProfile(user, "QuanLyThanhPham.QuanLyThanhPham.delete_owner"))
            {
                try
                {
                    var _QuanLyThanhPham = await _context.QuanLyThanhPham.FindAsync(id);
                    _QuanLyThanhPham.ModifiedDate = DateTime.Now;
                    _QuanLyThanhPham.ModifiedBy = HttpContext.User.Identity.Name;
                    _QuanLyThanhPham.Cancelled = true;

                    _context.Update(_QuanLyThanhPham);
                    await _context.SaveChangesAsync();

                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _QuanLyThanhPham;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Đã xóa thất bại";
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
            return _context.QuanLyThanhPham.Any(e => e.Id == id);
        }

        [HttpGet]
        public IActionResult GetAllThanhPham()
        {
            var rs = (from _tp in _context.QuanLyThanhPham
                      where _tp.Cancelled == false
                      select new
                      {
                          Id = _tp.Id,
                          TenThanhPham = _tp.TenThanhPham,
                          Mau = _tp.Mau,
                          PO = _tp.PO,
                          Size = _tp.Size,
                      }
                      ).ToList();
            return Ok(rs);
        }

        [Authorize]
        public IActionResult ChiTiet(int id)
        {
            var nl = _context.QuanLyThanhPham.FirstOrDefault(x => x.Id == id);

            return PartialView("_ChiTiet", nl);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetApiCustomfield()
        {


            var a = from _nl in _context.CustomField
                    select new
                    {
                        ID = _nl.ID,
                        FieldName = _nl.FieldName,
                    };
            return Ok(a.ToList());

        }
        [AllowAnonymous]

        [HttpGet]
        public IActionResult GetApiCustomfieldValue()
        {


            var a = from _nl in _context.CustomFieldInputValue
                    select new
                    {
                        ID = _nl.ID,
                        CustomFieldKey = _nl.CustomFieldKey,
                        CustomFieldValue = _nl.CustomFieldValue,
                    };
            return Ok(a.ToList());

        }

    }
}
