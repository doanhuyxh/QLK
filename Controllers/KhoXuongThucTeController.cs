using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.KhoXuongThucTeViewModel;
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
    public class KhoXuongThucTeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;

        public KhoXuongThucTeController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name ?? string.Empty;
        }


        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KhoXuongThucTe.KhoXuongThucTe.list"))
            {
                return View();
            }
            else if (_iCommon.CheckPermissionInProfile(user, "KhoXuongThucTe.KhoXuongThucTe.list_owner"))
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
                //NOT_IN_TREE_VIEW_START//NOT_IN_TREE_VIEW_END
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
        public IQueryable<KhoXuongThucTeCRUDViewModel> LoadKhoXuongThucTeChildrentByParentId(int ParentId, int ExcludeId = 0)
        {
            if (_iCommon.CheckPermissionInProfile(user, "KhoXuongThucTe.KhoXuongThucTe.list_owner"))
            {
                return (from _KhoXuongThucTe in _context.KhoXuongThucTe.Where(x => x.ParentId == ParentId && x.Id != ExcludeId && x.Cancelled == false && x.CreatedBy.Contains(user))
                .OrderBy(x => x.Id)
                        select new KhoXuongThucTeCRUDViewModel
                        {
                            Id = _KhoXuongThucTe.Id,
                            ParentId = _KhoXuongThucTe.ParentId,
                            TenKho = _KhoXuongThucTe.TenKho,
                            GhiChu = _KhoXuongThucTe.GhiChu,
                            ParentName = _context.KhoXuongThucTe.FirstOrDefault(x => x.Id == _KhoXuongThucTe.ParentId).TenKho ?? ""
                        });
            }

            return (from _KhoXuongThucTe in _context.KhoXuongThucTe.Where(x => x.ParentId == ParentId && x.Id != ExcludeId && x.Cancelled == false)
                .OrderBy(x => x.Id)
                    select new KhoXuongThucTeCRUDViewModel
                    {
                        Id = _KhoXuongThucTe.Id,
                        ParentId = _KhoXuongThucTe.ParentId,
                        TenKho = _KhoXuongThucTe.TenKho,
                        ParentName = _context.KhoXuongThucTe.FirstOrDefault(x => x.Id == _KhoXuongThucTe.ParentId).TenKho ?? ""
                    });
        }
        private List<KhoXuongThucTeCRUDViewModel> GetGridItem()
        {
            List<KhoXuongThucTeCRUDViewModel> KhoXuongThucTeCRUDViewModelList = new List<KhoXuongThucTeCRUDViewModel>();
            LoadKhoXuongThucTeChildrent(ref KhoXuongThucTeCRUDViewModelList, 0, 1);

            return KhoXuongThucTeCRUDViewModelList;
        }
        public void LoadKhoXuongThucTeChildrent(ref List<KhoXuongThucTeCRUDViewModel> KhoXuongThucTeCRUDViewModelList, int level = 0, int ParentId = 1, int ExcludeId = 0)
        {
            try
            {
                IQueryable<KhoXuongThucTeCRUDViewModel> _KhoXuongThucTeCRUDViewModelList = LoadKhoXuongThucTeChildrentByParentId(ParentId, ExcludeId);
                var level1 = level + 1;
                if (_KhoXuongThucTeCRUDViewModelList.Count() > 0)
                {
                    foreach (var item in _KhoXuongThucTeCRUDViewModelList)
                    {
                        item.TenKho = "---".Repeat((uint)level) + item.TenKho;
                        KhoXuongThucTeCRUDViewModelList.Add(item);
                        LoadKhoXuongThucTeChildrent(ref KhoXuongThucTeCRUDViewModelList, level1, item.Id, ExcludeId);
                    }
                }

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
            KhoXuongThucTeCRUDViewModel vm = await _context.KhoXuongThucTe.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            KhoXuongThucTeCRUDViewModel vm = new KhoXuongThucTeCRUDViewModel();
            List<KhoXuongThucTeCRUDViewModel> KhoXuongThucTeCRUDViewModelList = new List<KhoXuongThucTeCRUDViewModel>();
            KhoXuongThucTeCRUDViewModelList.Add(new KhoXuongThucTeCRUDViewModel
            {
                Id = 1,
                ParentId = 0,
                TenKho = "Root",
            });
            if (id > 0)
            {
                LoadKhoXuongThucTe(ref KhoXuongThucTeCRUDViewModelList, 1, 1, id);
            }
            else
            {
                LoadKhoXuongThucTe(ref KhoXuongThucTeCRUDViewModelList, 1, 1);
            }
            ViewBag._LoadKhoXuongThucTe = new SelectList(KhoXuongThucTeCRUDViewModelList, "Id", "TenKho");

            if (id > 0)
            {
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KhoXuongThucTe.KhoXuongThucTe.edit"))
                {
                    vm = await _context.KhoXuongThucTe.Where(x => x.Id == id).SingleOrDefaultAsync();
                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "KhoXuongThucTe.KhoXuongThucTe.edit_owner"))
                {
                    vm = await _context.KhoXuongThucTe.Where(x => x.Id == id && x.CreatedBy.Contains(user)).SingleOrDefaultAsync();
                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KhoXuongThucTe.KhoXuongThucTe.create"))
                {
                    return PartialView("_AddEdit", vm);

                }

            }
            return View("_AccessDeniedAddEdit");

        }
        public IQueryable<KhoXuongThucTeCRUDViewModel> LoadKhoXuongThucTeByParentId(int ParentId, int ExcludeId = 0)
        {
            return (from _KhoXuongThucTe in _context.KhoXuongThucTe.Where(x => x.ParentId == ParentId && x.Id != ExcludeId && x.Cancelled == false)
                .OrderBy(x => x.Id)
                    select new KhoXuongThucTeCRUDViewModel
                    {
                        Id = _KhoXuongThucTe.Id,
                        ParentId = _KhoXuongThucTe.ParentId,
                        TenKho = _KhoXuongThucTe.TenKho
                    });
        }
        public void LoadKhoXuongThucTe(ref List<KhoXuongThucTeCRUDViewModel> KhoXuongThucTeCRUDViewModelList, int level = 0, int ParentId = 1, int ExcludeId = 0)
        {
            try
            {
                IQueryable<KhoXuongThucTeCRUDViewModel> _KhoXuongThucTeCRUDViewModelList = LoadKhoXuongThucTeByParentId(ParentId, ExcludeId);
                var level1 = level + 1;
                if (_KhoXuongThucTeCRUDViewModelList.Count() > 0)
                {
                    foreach (var item in _KhoXuongThucTeCRUDViewModelList)
                    {
                        item.TenKho = "---".Repeat((uint)level) + item.TenKho;
                        KhoXuongThucTeCRUDViewModelList.Add(item);
                        LoadKhoXuongThucTe(ref KhoXuongThucTeCRUDViewModelList, level1, item.Id, ExcludeId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> AddEdit(KhoXuongThucTeCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    KhoXuongThucTe _KhoXuongThucTe = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _KhoXuongThucTe = await _context.KhoXuongThucTe.FindAsync(vm.Id);


                        vm.CreatedDate = _KhoXuongThucTe.CreatedDate;
                        vm.CreatedBy = _KhoXuongThucTe.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_KhoXuongThucTe).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "Kho/Xưởng: " + vm.TenKho + " đã cập nhật thành công";
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _KhoXuongThucTe = vm;


                        _KhoXuongThucTe.CreatedDate = DateTime.Now;
                        _KhoXuongThucTe.ModifiedDate = DateTime.Now;
                        _KhoXuongThucTe.CreatedBy = _UserName;
                        _KhoXuongThucTe.ModifiedBy = _UserName;
                        _context.Add(_KhoXuongThucTe);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "Kho/Xuong: " + vm.TenKho + " đã tạo thành công";
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
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "KhoXuongThucTe.KhoXuongThucTe.delete"))
            {
                try
                {
                    var _KhoXuongThucTe = await _context.KhoXuongThucTe.FindAsync(id);
                    _KhoXuongThucTe.ModifiedDate = DateTime.Now;
                    _KhoXuongThucTe.ModifiedBy = HttpContext.User.Identity.Name;
                    _KhoXuongThucTe.Cancelled = true;

                    _context.Update(_KhoXuongThucTe);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _KhoXuongThucTe;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Xóa không thành công";
                    _JsonResultViewModel.IsSuccess = false;
                    _JsonResultViewModel.ModelObject = null;
                    return new JsonResult(_JsonResultViewModel);
                }
            }
            else if (_iCommon.CheckPermissionInProfile(user, "KhoXuongThucTe.KhoXuongThucTe.delete_owner"))
            {
                try
                {
                    var _KhoXuongThucTe = await _context.KhoXuongThucTe.FindAsync(id);
                    _KhoXuongThucTe.ModifiedDate = DateTime.Now;
                    _KhoXuongThucTe.ModifiedBy = HttpContext.User.Identity.Name;
                    _KhoXuongThucTe.Cancelled = true;

                    _context.Update(_KhoXuongThucTe);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _KhoXuongThucTe;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Xóa không thành công";
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
        public IActionResult GetApiKho()
        {
            var a = from _kho in _context.KhoXuongThucTe
                    where _kho.Cancelled == false
                    select new
                    {
                        Id = _kho.Id,
                        TenKho = _kho.TenKho,
                        GhiChu = _kho.GhiChu,
                        ParentId = _kho.ParentId,
                    };

            return Ok(a.ToList());
        }

        private bool IsExists(long id)
        {
            return _context.KhoXuongThucTe.Any(e => e.Id == id);
        }

    }
}

