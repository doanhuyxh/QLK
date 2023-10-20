using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.ObjectFieldViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ObjectFieldController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public ObjectFieldController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        //[Authorize(Roles = AMS.Pages.MainMenu.ObjectField.RoleName)]
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
                    || obj.KieuDuLieu.ToLower().Contains(searchValue)
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

        private IQueryable<ObjectFieldCRUDViewModel> GetGridItem()
        {
            try
            {
                return (from _ObjectField in _context.ObjectField
                        where _ObjectField.Cancelled == false
                        select new ObjectFieldCRUDViewModel
                        {
                            Id = _ObjectField.Id,
                            TenTruong = _ObjectField.TenTruong,
                            RowIndex = _ObjectField.RowIndex,
                            KieuDuLieu = _ObjectField.KieuDuLieu,
                            CreatedDate = _ObjectField.CreatedDate,
                            ModifiedDate = _ObjectField.ModifiedDate,
                            CreatedBy = _ObjectField.CreatedBy,
                            ModifiedBy = _ObjectField.ModifiedBy,

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
            ObjectFieldCRUDViewModel vm = await _context.ObjectField.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            ObjectFieldCRUDViewModel vm = new ObjectFieldCRUDViewModel();
            if (id > 0) vm = await _context.ObjectField.Where(x => x.Id == id).SingleOrDefaultAsync();
            return PartialView("_AddEdit", vm);
        }

        [HttpPost]
        public async Task<JsonResult> AddEdit(ObjectFieldCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ObjectField _ObjectField = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _ObjectField = await _context.ObjectField.FindAsync(vm.Id);

                        vm.CreatedDate = _ObjectField.CreatedDate;
                        vm.CreatedBy = _ObjectField.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_ObjectField).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "ObjectField đã cập nhật thành công ID: " + _ObjectField.Id;
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _ObjectField = vm;
                        _ObjectField.CreatedDate = DateTime.Now;
                        _ObjectField.ModifiedDate = DateTime.Now;
                        _ObjectField.CreatedBy = _UserName;
                        _ObjectField.ModifiedBy = _UserName;
                        _context.Add(_ObjectField);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "ObjectField đã thêm thành công ID: " + _ObjectField.Id;
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
                var _ObjectField = await _context.ObjectField.FindAsync(id);
                _ObjectField.ModifiedDate = DateTime.Now;
                _ObjectField.ModifiedBy = HttpContext.User.Identity.Name;
                _ObjectField.Cancelled = true;

                _context.Update(_ObjectField);
                await _context.SaveChangesAsync();
                return new JsonResult(_ObjectField);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.ObjectField.Any(e => e.Id == id);
        }
    }
}
