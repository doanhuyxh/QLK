using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.KiemKhoViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
using AMS.Models.NguyenLieuKiemKhoViewModel;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class KiemKhoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;

        public KiemKhoController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name ?? string.Empty;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KiemKho.KiemKho.list"))
            {
                return View();
            }
            else if (_iCommon.CheckPermissionInProfile(user, "KiemKho.KiemKho.list_owner"))
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
                     || obj.Name.ToLower().Contains(searchValue)
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
        private IQueryable<KiemKhoCRUDViewModel> GetGridItem()
        {
            if (_iCommon.CheckPermissionInProfile(user, "KiemKho.KiemKho.list_owner"))
            {
                try
                {
                    return (from _KiemKho in _context.KiemKho
                            where _KiemKho.Cancelled == false && _KiemKho.CreatedBy.Contains(user)

                            select new KiemKhoCRUDViewModel
                            {
                                Id = _KiemKho.Id,
                                Name = _KiemKho.Name,
                                NhanVienId = _KiemKho.NhanVienId,
                                NgayKiem = _KiemKho.NgayKiem,
                                //KhoId = _KiemKho.KhoId,
                                CreatedDate = _KiemKho.CreatedDate,
                                ModifiedDate = _KiemKho.ModifiedDate,
                                CreatedBy = _KiemKho.CreatedBy,
                                ModifiedBy = _KiemKho.ModifiedBy,

                            }).OrderByDescending(x => x.Id);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            try
            {
                return (from _KiemKho in _context.KiemKho
                        where _KiemKho.Cancelled == false

                        select new KiemKhoCRUDViewModel
                        {
                            Id = _KiemKho.Id,
                            Name = _KiemKho.Name,
                            NhanVienId = _KiemKho.NhanVienId,
                            NgayKiem = _KiemKho.NgayKiem,
                            //KhoId = _KiemKho.KhoId,
                            CreatedDate = _KiemKho.CreatedDate,
                            ModifiedDate = _KiemKho.ModifiedDate,
                            CreatedBy = _KiemKho.CreatedBy,
                            ModifiedBy = _KiemKho.ModifiedBy,

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
            KiemKhoCRUDViewModel vm = await _context.KiemKho.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            KiemKhoCRUDViewModel vm = new KiemKhoCRUDViewModel();
            if (id > 0)
            {
                vm = await _context.KiemKho.Where(x => x.Id == id).SingleOrDefaultAsync();
                ViewBag._LoadNlKk = JsonConvert.SerializeObject(GetNguyenLieuKiemKhoByKiemKhoId(id));

                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KiemKho.KiemKho.edit"))
                {
                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "KiemKho.KiemKho.edit_owner"))
                {
                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KiemKho.KiemKho.create"))
                {
                    ViewBag._LoadNlKk = JsonConvert.SerializeObject(Array.Empty<NguyenLieuKiemKhoViewModel>());
                    return PartialView("_AddEdit", vm);
                }
            }
            return View("_AccessDeniedAddEdit");
        }

        private List<NguyenLieuKiemKhoViewModel> GetNguyenLieuKiemKhoByKiemKhoId(int idKiemKho)
        {
            var x = from _nlkk in _context.NguyenLieuKiemKho
                    where _nlkk.KiemKhoId == idKiemKho & _nlkk.Cancelled == false
                    select new NguyenLieuKiemKhoViewModel
                    {
                        Id = _nlkk.Id,
                        SoLuongThucTe = _nlkk.SoLuongThucTe,
                        KiemKhoId = idKiemKho,
                        NguyenLieuId = _nlkk.NguyenLieuId,
                        ChatLuong = _nlkk.ChatLuong,
                    };
            return x.ToList();
        }
        [HttpPost]
        public async Task<JsonResult> AddEdit(KiemKhoCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    KiemKho _KiemKho = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _KiemKho = await _context.KiemKho.FindAsync(vm.Id);


                        vm.CreatedDate = _KiemKho.CreatedDate;
                        vm.CreatedBy = _KiemKho.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_KiemKho).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();
                        foreach (var item in vm.NguyenLieuKiemKho)
                        {
                            if (item.Id > 0)
                            {
                                var nk = await _context.NguyenLieuKiemKho.FindAsync(item.Id);
                                _context.Entry(nk).CurrentValues.SetValues(item);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                NguyenLieuKiemKho nlkk = item;
                                nlkk.KiemKhoId = _KiemKho.Id;
                                nlkk.CreatedDate = DateTime.Now;
                                nlkk.ModifiedDate = DateTime.Now;
                                nlkk.CreatedBy = _UserName;
                                nlkk.ModifiedBy = _UserName;
                                _context.Add(nlkk);
                                await _context.SaveChangesAsync();
                            }
                        }
                        var _AlertMessage = "Kiểm kho đã cập nhật thành công";
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _KiemKho = vm;


                        _KiemKho.CreatedDate = DateTime.Now;
                        _KiemKho.ModifiedDate = DateTime.Now;
                        _KiemKho.CreatedBy = _UserName;
                        _KiemKho.ModifiedBy = _UserName;
                        _context.Add(_KiemKho);
                        await _context.SaveChangesAsync();
                        foreach (var item in vm.NguyenLieuKiemKho)
                        {
                            //ghi nguyên liệu cần kiểm và bảng
                            NguyenLieuKiemKho nlkk = item;
                            nlkk.KiemKhoId = _KiemKho.Id;
                            nlkk.CreatedDate = DateTime.Now;
                            nlkk.ModifiedDate = DateTime.Now;
                            nlkk.CreatedBy = _UserName;
                            nlkk.ModifiedBy = _UserName;
                            _context.Add(nlkk);
                            await _context.SaveChangesAsync();

                            //cập lại số lượng của nguyên liệu trong kho cho đúng số lượng thực tế

                        }
                        var _AlertMessage = "Kiểm kho đã tạo mới thành công";
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

            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KiemKho.KiemKho.delete"))
            {
                try
                {
                    var _KiemKho = await _context.KiemKho.FindAsync(id);
                    _KiemKho.ModifiedDate = DateTime.Now;
                    _KiemKho.ModifiedBy = HttpContext.User.Identity.Name;
                    _KiemKho.Cancelled = true;

                    _context.Update(_KiemKho);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _KiemKho;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else if (_iCommon.CheckPermissionInProfile(user, "KiemKho.KiemKho.delete_owner"))
            {
                try
                {
                    var _KiemKho = await _context.KiemKho.FindAsync(id);
                    _KiemKho.ModifiedDate = DateTime.Now;
                    _KiemKho.ModifiedBy = HttpContext.User.Identity.Name;
                    _KiemKho.Cancelled = true;

                    _context.Update(_KiemKho);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _KiemKho;
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
        public IActionResult GetNguoiDung()
        {
            var a = _context.UserProfile.Where(x => x.Cancelled == false).ToList();
            return Json(a);
        }

        private bool IsExists(long id)
        {
            return _context.KiemKho.Any(e => e.Id == id);
        }
    }
}
