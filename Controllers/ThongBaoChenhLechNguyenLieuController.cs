using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.ThongBaoChenhLechNguyenLieuViewModel;
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
    public class ThongBaoChenhLechNguyenLieuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;

        public ThongBaoChenhLechNguyenLieuController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name ?? string.Empty;
        }

        //[Authorize(Roles = AMS.Pages.MainMenu.ThongBaoChenhLechNguyenLieu.RoleName)]
        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "ThongBaoChenhLechNguyenLieu.ThongBaoChenhLechNguyenLieu.list"))
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
                    || obj.NoiDung.ToLower().Contains(searchValue)
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
        private IQueryable<ThongBaoChenhLechNguyenLieuCRUDViewModel> GetGridItem()
        {
            try
            {
                return (from _ThongBaoChenhLechNguyenLieu in _context.ThongBaoChenhLechNguyenLieu
                        where _ThongBaoChenhLechNguyenLieu.Cancelled == false
                        
                        select new ThongBaoChenhLechNguyenLieuCRUDViewModel
                        {
                            Id = _ThongBaoChenhLechNguyenLieu.Id,
                            NoiDung = _ThongBaoChenhLechNguyenLieu.NoiDung,
                            CreatedDate = _ThongBaoChenhLechNguyenLieu.CreatedDate,
                            ModifiedDate = _ThongBaoChenhLechNguyenLieu.ModifiedDate,
                            CreatedBy = _ThongBaoChenhLechNguyenLieu.CreatedBy,
                            ModifiedBy = _ThongBaoChenhLechNguyenLieu.ModifiedBy,

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
            ThongBaoChenhLechNguyenLieuCRUDViewModel vm = await _context.ThongBaoChenhLechNguyenLieu.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            ThongBaoChenhLechNguyenLieuCRUDViewModel vm = new ThongBaoChenhLechNguyenLieuCRUDViewModel();
            if (id > 0) vm = await _context.ThongBaoChenhLechNguyenLieu.Where(x => x.Id == id).SingleOrDefaultAsync();
            
            //[INSERT_OBJECTSUBVIEWFIELDLIST]
            return PartialView("_AddEdit", vm);
        }
        //[INSERT_LOAD]

        [HttpPost]
        public async Task<JsonResult> AddEdit(ThongBaoChenhLechNguyenLieuCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ThongBaoChenhLechNguyenLieu _ThongBaoChenhLechNguyenLieu = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _ThongBaoChenhLechNguyenLieu = await _context.ThongBaoChenhLechNguyenLieu.FindAsync(vm.Id);
                        

                        vm.CreatedDate = _ThongBaoChenhLechNguyenLieu.CreatedDate;
                        vm.CreatedBy = _ThongBaoChenhLechNguyenLieu.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_ThongBaoChenhLechNguyenLieu).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();
                        
                        var _AlertMessage = "ThongBaoChenhLechNguyenLieu đã cập nhật thành công ID: " + _ThongBaoChenhLechNguyenLieu.Id;
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _ThongBaoChenhLechNguyenLieu = vm;
                        

                        _ThongBaoChenhLechNguyenLieu.CreatedDate = DateTime.Now;
                        _ThongBaoChenhLechNguyenLieu.ModifiedDate = DateTime.Now;
                        _ThongBaoChenhLechNguyenLieu.CreatedBy = _UserName;
                        _ThongBaoChenhLechNguyenLieu.ModifiedBy = _UserName;
                        _context.Add(_ThongBaoChenhLechNguyenLieu);
                        await _context.SaveChangesAsync();
                        
                        var _AlertMessage = "ThongBaoChenhLechNguyenLieu đã thêm thành công ID: " + _ThongBaoChenhLechNguyenLieu.Id;
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
            try
            {
                var _ThongBaoChenhLechNguyenLieu = await _context.ThongBaoChenhLechNguyenLieu.FindAsync(id);
                _ThongBaoChenhLechNguyenLieu.ModifiedDate = DateTime.Now;
                _ThongBaoChenhLechNguyenLieu.ModifiedBy = HttpContext.User.Identity.Name;
                _ThongBaoChenhLechNguyenLieu.Cancelled = true;

                _context.Update(_ThongBaoChenhLechNguyenLieu);
                await _context.SaveChangesAsync();
                return new JsonResult(_ThongBaoChenhLechNguyenLieu);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.ThongBaoChenhLechNguyenLieu.Any(e => e.Id == id);
        }
    }
}
