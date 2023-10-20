using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.QuanLyMenuTypeViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class QuanLyMenuTypeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public QuanLyMenuTypeController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        //[Authorize(Roles = AMS.Pages.MainMenu.QuanLyMenuType.RoleName)]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
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
                    || obj.TenMenuType.ToLower().Contains(searchValue)
                    || obj.MoTa.ToLower().Contains(searchValue)
                    || obj.CreatedDate.ToString().ToLower().Contains(searchValue)
                    || obj.ModifiedDate.ToString().ToLower().Contains(searchValue)
                    || obj.CreatedBy.ToLower().Contains(searchValue)
                    || obj.ModifiedBy.ToLower().Contains(searchValue)

                    || obj.CreatedDate.ToString().Contains(searchValue));
                }

                resultTotal = _GetGridItem.Count();

                var result = _GetGridItem.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = resultTotal, recordsTotal = resultTotal, data = result });

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private IQueryable<QuanLyMenuTypeCRUDViewModel> GetGridItem()
        {
            try
            {
                return (from _QuanLyMenuType in _context.QuanLyMenuType
                        where _QuanLyMenuType.Cancelled == false

                        select new QuanLyMenuTypeCRUDViewModel
                        {
                            Id = _QuanLyMenuType.Id,
                            TenMenuType = _QuanLyMenuType.TenMenuType,
                            MoTa = _QuanLyMenuType.MoTa,
                            LaMenuFrontend = _QuanLyMenuType.LaMenuFrontend,
                            CreatedDate = _QuanLyMenuType.CreatedDate,
                            ModifiedDate = _QuanLyMenuType.ModifiedDate,
                            CreatedBy = _QuanLyMenuType.CreatedBy,
                            ModifiedBy = _QuanLyMenuType.ModifiedBy,

                        }).OrderByDescending(x => x.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();
            QuanLyMenuTypeCRUDViewModel vm = await _context.QuanLyMenuType.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            QuanLyMenuTypeCRUDViewModel vm = new QuanLyMenuTypeCRUDViewModel();
            if (id > 0) vm = await _context.QuanLyMenuType.Where(x => x.Id == id).SingleOrDefaultAsync();

            return PartialView("_AddEdit", vm);
        }

        [HttpPost]
        public async Task<JsonResult> AddEdit(QuanLyMenuTypeCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    QuanLyMenuType _QuanLyMenuType = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _QuanLyMenuType = await _context.QuanLyMenuType.FindAsync(vm.Id);

                        vm.CreatedDate = _QuanLyMenuType.CreatedDate;
                        vm.CreatedBy = _QuanLyMenuType.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_QuanLyMenuType).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "QuanLyMenuType đã cập nhật thành công ID: " + _QuanLyMenuType.Id;
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _QuanLyMenuType = vm;
                        _QuanLyMenuType.CreatedDate = DateTime.Now;
                        _QuanLyMenuType.ModifiedDate = DateTime.Now;
                        _QuanLyMenuType.CreatedBy = _UserName;
                        _QuanLyMenuType.ModifiedBy = _UserName;
                        _context.Add(_QuanLyMenuType);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "QuanLyMenuType đã thêm thành công ID: " + _QuanLyMenuType.Id;
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
                var _QuanLyMenuType = await _context.QuanLyMenuType.FindAsync(id);
                _QuanLyMenuType.ModifiedDate = DateTime.Now;
                _QuanLyMenuType.ModifiedBy = HttpContext.User.Identity.Name;
                _QuanLyMenuType.Cancelled = true;

                _context.Update(_QuanLyMenuType);
                await _context.SaveChangesAsync();
                return new JsonResult(_QuanLyMenuType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.QuanLyMenuType.Any(e => e.Id == id);
        }
    }
}
