using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.DisplayFieldOfTableViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class DisplayFieldOfTableController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public DisplayFieldOfTableController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        //[Authorize(Roles = AMS.Pages.MainMenu.DisplayFieldOfTable.RoleName)]
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
                    || obj.TenTruong.ToLower().Contains(searchValue)
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

        private IQueryable<DisplayFieldOfTableCRUDViewModel> GetGridItem()
        {
            try
            {
                return (from _DisplayFieldOfTable in _context.DisplayFieldOfTable
                        where _DisplayFieldOfTable.Cancelled == false
                        select new DisplayFieldOfTableCRUDViewModel
                        {
                            Id = _DisplayFieldOfTable.Id,
                            TenTruong = _DisplayFieldOfTable.TenTruong,
                            MoTa = _DisplayFieldOfTable.MoTa,
                            CreatedDate = _DisplayFieldOfTable.CreatedDate,
                            ModifiedDate = _DisplayFieldOfTable.ModifiedDate,
                            CreatedBy = _DisplayFieldOfTable.CreatedBy,
                            ModifiedBy = _DisplayFieldOfTable.ModifiedBy,

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
            DisplayFieldOfTableCRUDViewModel vm = await _context.DisplayFieldOfTable.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            DisplayFieldOfTableCRUDViewModel vm = new DisplayFieldOfTableCRUDViewModel();
            if (id > 0) vm = await _context.DisplayFieldOfTable.Where(x => x.Id == id).SingleOrDefaultAsync();
            return PartialView("_AddEdit", vm);
        }

        [HttpPost]
        public async Task<JsonResult> AddEdit(DisplayFieldOfTableCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DisplayFieldOfTable _DisplayFieldOfTable = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _DisplayFieldOfTable = await _context.DisplayFieldOfTable.FindAsync(vm.Id);

                        vm.CreatedDate = _DisplayFieldOfTable.CreatedDate;
                        vm.CreatedBy = _DisplayFieldOfTable.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_DisplayFieldOfTable).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "DisplayFieldOfTable đã cập nhật thành công ID: " + _DisplayFieldOfTable.Id;
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _DisplayFieldOfTable = vm;
                        _DisplayFieldOfTable.CreatedDate = DateTime.Now;
                        _DisplayFieldOfTable.ModifiedDate = DateTime.Now;
                        _DisplayFieldOfTable.CreatedBy = _UserName;
                        _DisplayFieldOfTable.ModifiedBy = _UserName;
                        _context.Add(_DisplayFieldOfTable);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "DisplayFieldOfTable đã thêm thành công ID: " + _DisplayFieldOfTable.Id;
                        return new JsonResult(_AlertMessage);
                    }
                }
                return new JsonResult("Operation failed.");
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
                var _DisplayFieldOfTable = await _context.DisplayFieldOfTable.FindAsync(id);
                _DisplayFieldOfTable.ModifiedDate = DateTime.Now;
                _DisplayFieldOfTable.ModifiedBy = HttpContext.User.Identity.Name;
                _DisplayFieldOfTable.Cancelled = true;

                _context.Update(_DisplayFieldOfTable);
                await _context.SaveChangesAsync();
                return new JsonResult(_DisplayFieldOfTable);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.DisplayFieldOfTable.Any(e => e.Id == id);
        }
    }
}
