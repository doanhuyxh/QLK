using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.ViewItemTemplateViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
//[INSERT_USING]
namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ViewItemTemplateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public ViewItemTemplateController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        //[Authorize(Roles = AMS.Pages.MainMenu.ViewItemTemplate.RoleName)]
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
                    //[INSERT_FIELD_CONTROLLER_1]
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
        private IQueryable<ViewItemTemplateCRUDViewModel> GetGridItem()
        {
            try
            {
                return (from _ViewItemTemplate in _context.ViewItemTemplate
                        where _ViewItemTemplate.Cancelled == false
                        //[INSERT_FIELD_LEFT_JOINT_CONTROLLER]
                        select new ViewItemTemplateCRUDViewModel
                        {
                            Id = _ViewItemTemplate.Id,
                            //[INSERT_FIELD_CONTROLLER_2]
                            CreatedDate = _ViewItemTemplate.CreatedDate,
                            ModifiedDate = _ViewItemTemplate.ModifiedDate,
                            CreatedBy = _ViewItemTemplate.CreatedBy,
                            ModifiedBy = _ViewItemTemplate.ModifiedBy,

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
            ViewItemTemplateCRUDViewModel vm = await _context.ViewItemTemplate.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            ViewItemTemplateCRUDViewModel vm = new ViewItemTemplateCRUDViewModel();
            if (id > 0) vm = await _context.ViewItemTemplate.Where(x => x.Id == id).SingleOrDefaultAsync();
            //[INSERT_OBJECTFIELDLIST]
            //[INSERT_OBJECTSUBVIEWFIELDLIST]
            return PartialView("_AddEdit", vm);
        }
        //[INSERT_LOAD]
        [HttpPost]
        public async Task<JsonResult> AddEdit(ViewItemTemplateCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ViewItemTemplate _ViewItemTemplate = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _ViewItemTemplate = await _context.ViewItemTemplate.FindAsync(vm.Id);
                        //[INSERT_SAVE_FIELD_IMAGE_CONTROLLER]

                        vm.CreatedDate = _ViewItemTemplate.CreatedDate;
                        vm.CreatedBy = _ViewItemTemplate.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_ViewItemTemplate).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();
                        //[INSERT_SAVE_FIELD_SUB_ITEM_VIEW_CONTROLLER]
                        var _AlertMessage = "ViewItemTemplate đã cập nhật thành công ID: " + _ViewItemTemplate.Id;
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _ViewItemTemplate = vm;
                        //[INSERT_INSERT_FIELD_IMAGE_CONTROLLER]

                        _ViewItemTemplate.CreatedDate = DateTime.Now;
                        _ViewItemTemplate.ModifiedDate = DateTime.Now;
                        _ViewItemTemplate.CreatedBy = _UserName;
                        _ViewItemTemplate.ModifiedBy = _UserName;
                        _context.Add(_ViewItemTemplate);
                        await _context.SaveChangesAsync();
                        //[INSERT_SAVE_INSERT_FIELD_SUB_ITEM_VIEW_CONTROLLER]
                        var _AlertMessage = "ViewItemTemplate đã thêm thành công ID: " + _ViewItemTemplate.Id;
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
                var _ViewItemTemplate = await _context.ViewItemTemplate.FindAsync(id);
                _ViewItemTemplate.ModifiedDate = DateTime.Now;
                _ViewItemTemplate.ModifiedBy = HttpContext.User.Identity.Name;
                _ViewItemTemplate.Cancelled = true;

                _context.Update(_ViewItemTemplate);
                await _context.SaveChangesAsync();
                return new JsonResult(_ViewItemTemplate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.ViewItemTemplate.Any(e => e.Id == id);
        }
    }
}
