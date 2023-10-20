using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.RelationshipTableViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;


//[INSERT_USING]
namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class RelationshipTableController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public RelationshipTableController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        //[Authorize(Roles = AMS.Pages.MainMenu.RelationshipTable.RoleName)]
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
                    || obj.TenBang.ToLower().Contains(searchValue)
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

        private IQueryable<RelationshipTableCRUDViewModel> GetGridItem()
        {
            try
            {
                return (from _RelationshipTable in _context.RelationshipTable
                        where _RelationshipTable.Cancelled == false

                        select new RelationshipTableCRUDViewModel
                        {
                            Id = _RelationshipTable.Id,
                            TenBang = _RelationshipTable.TenBang,
                            MoTa = _RelationshipTable.MoTa,
                            CreatedDate = _RelationshipTable.CreatedDate,
                            ModifiedDate = _RelationshipTable.ModifiedDate,
                            CreatedBy = _RelationshipTable.CreatedBy,
                            ModifiedBy = _RelationshipTable.ModifiedBy,

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
            RelationshipTableCRUDViewModel vm = await _context.RelationshipTable.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            RelationshipTableCRUDViewModel vm = new RelationshipTableCRUDViewModel();
            if (id > 0) vm = await _context.RelationshipTable.Where(x => x.Id == id).SingleOrDefaultAsync();


            //[INSERT_OBJECTFIELDLIST]
            return PartialView("_AddEdit", vm);
        }


        //[INSERT_LOAD]
        [HttpPost]
        public async Task<JsonResult> AddEdit(RelationshipTableCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RelationshipTable _RelationshipTable = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _RelationshipTable = await _context.RelationshipTable.FindAsync(vm.Id);

                        vm.CreatedDate = _RelationshipTable.CreatedDate;
                        vm.CreatedBy = _RelationshipTable.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_RelationshipTable).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "RelationshipTable đã cập nhật thành công ID: " + _RelationshipTable.Id;
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _RelationshipTable = vm;
                        _RelationshipTable.CreatedDate = DateTime.Now;
                        _RelationshipTable.ModifiedDate = DateTime.Now;
                        _RelationshipTable.CreatedBy = _UserName;
                        _RelationshipTable.ModifiedBy = _UserName;
                        _context.Add(_RelationshipTable);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "RelationshipTable đã thêm thành công ID: " + _RelationshipTable.Id;
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
                var _RelationshipTable = await _context.RelationshipTable.FindAsync(id);
                _RelationshipTable.ModifiedDate = DateTime.Now;
                _RelationshipTable.ModifiedBy = HttpContext.User.Identity.Name;
                _RelationshipTable.Cancelled = true;

                _context.Update(_RelationshipTable);
                await _context.SaveChangesAsync();
                return new JsonResult(_RelationshipTable);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.RelationshipTable.Any(e => e.Id == id);
        }
    }
}
