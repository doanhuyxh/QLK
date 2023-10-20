using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.ConfigViewModel;
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
    public class ConfigController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public ConfigController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        //[Authorize(Roles = AMS.Pages.MainMenu.Config.RoleName)]
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
                    || obj.Config1.ToLower().Contains(searchValue)
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
        private IQueryable<ConfigCRUDViewModel> GetGridItem()
        {
            try
            {
                return (from _Config in _context.Config
                        where _Config.Cancelled == false
                        
                        select new ConfigCRUDViewModel
                        {
                            Id = _Config.Id,
                            Config1 = _Config.Config1,
                            CreatedDate = _Config.CreatedDate,
                            ModifiedDate = _Config.ModifiedDate,
                            CreatedBy = _Config.CreatedBy,
                            ModifiedBy = _Config.ModifiedBy,

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
            ConfigCRUDViewModel vm = await _context.Config.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            ConfigCRUDViewModel vm = new ConfigCRUDViewModel();
            if (id > 0) vm = await _context.Config.Where(x => x.Id == id).SingleOrDefaultAsync();
            
            //[INSERT_OBJECTSUBVIEWFIELDLIST]
            return PartialView("_AddEdit", vm);
        }
        //[INSERT_LOAD]

        [HttpPost]
        public async Task<JsonResult> AddEdit(ConfigCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Config _Config = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _Config = await _context.Config.FindAsync(vm.Id);
                        

                        vm.CreatedDate = _Config.CreatedDate;
                        vm.CreatedBy = _Config.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_Config).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();
                        
                        var _AlertMessage = "Config đã cập nhật thành công ID: " + _Config.Id;
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _Config = vm;
                        

                        _Config.CreatedDate = DateTime.Now;
                        _Config.ModifiedDate = DateTime.Now;
                        _Config.CreatedBy = _UserName;
                        _Config.ModifiedBy = _UserName;
                        _context.Add(_Config);
                        await _context.SaveChangesAsync();
                        
                        var _AlertMessage = "Config đã thêm thành công ID: " + _Config.Id;
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
                var _Config = await _context.Config.FindAsync(id);
                _Config.ModifiedDate = DateTime.Now;
                _Config.ModifiedBy = HttpContext.User.Identity.Name;
                _Config.Cancelled = true;

                _context.Update(_Config);
                await _context.SaveChangesAsync();
                return new JsonResult(_Config);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.Config.Any(e => e.Id == id);
        }
    }
}
