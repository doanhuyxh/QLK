using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.QuanLyMenuViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using AMS.Helpers;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using AMS.Models.CommonViewModel;
using System.Linq;
using log4net;
using System.Runtime.InteropServices.WindowsRuntime;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class QuanLyMenuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public QuanLyMenuController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        //[Authorize(Roles = AMS.Pages.MainMenu.QuanLyMenu.RoleName)]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ReSort()
        {
            await AReSort(1);
            JsonResultViewModel _JsonResultViewModel = new();
            try
            {
                var _GetGridItem = GetGridItem();
                _JsonResultViewModel.AlertMessage = "lấy dữ liệu thành công";
                _JsonResultViewModel.IsSuccess = true;
                _JsonResultViewModel.ModelObject = _GetGridItem;
                return new JsonResult(_JsonResultViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task AReSort(int Id)
        {
            List<QuanLyMenuCRUDViewModel> ListMenuItem = _iCommon.LoadQuanLyMenuByParentId(Id).ToList();
            for (var i = 0; i < ListMenuItem.Count(); i++)
            {
                var a_MenuItem = ListMenuItem[i];
                var _QuanLyMenu = await _context.QuanLyMenu.FindAsync(a_MenuItem.Id);
                _context.Entry(_QuanLyMenu).Property(u => u.Ordering).CurrentValue = i + 1;
                await _context.SaveChangesAsync();
                await AReSort(a_MenuItem.Id);
            }
        }

        [HttpGet]
        public async Task<IActionResult> MenuUp()
        {
            /*
            List<int> nums = new List<int>() { 1, 2, 3, 4, 5 };

            nums.Swap(2, 3);
            */
            int Id =int.Parse(HttpContext.Request.Query["Id"].ToString());
            int ordering = int.Parse(HttpContext.Request.Query["ordering"].ToString());
            QuanLyMenuCRUDViewModel MenuItem =await _context.QuanLyMenu.FirstOrDefaultAsync(m => m.Id == Id);
            List<QuanLyMenuCRUDViewModel> ListMenuItem =  _iCommon.LoadQuanLyMenuByParentId(MenuItem.ParentId).ToList();
            var index = ordering - 1;
            ListMenuItem.Swap(index - 1, index);
            for (var i = 0; i < ListMenuItem.Count(); i++)
            {
                var a_MenuItem = ListMenuItem[i];
                var _QuanLyMenu = await _context.QuanLyMenu.FindAsync(a_MenuItem.Id);
                _context.Entry(_QuanLyMenu).Property(u => u.Ordering).CurrentValue = i + 1;
                await _context.SaveChangesAsync();
            }
            
            JsonResultViewModel _JsonResultViewModel = new();

            try
            {
                var _GetGridItem = GetGridItem();
                _JsonResultViewModel.AlertMessage = "lấy dữ liệu thành công";
                _JsonResultViewModel.IsSuccess = true;
                _JsonResultViewModel.ModelObject = _GetGridItem;
                return new JsonResult(_JsonResultViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public async Task<IActionResult> MenuDown()
        {
            /*
            List<int> nums = new List<int>() { 1, 2, 3, 4, 5 };

            nums.Swap(2, 3);
            */
            int Id = int.Parse(HttpContext.Request.Query["Id"].ToString());
            int ordering = int.Parse(HttpContext.Request.Query["ordering"].ToString());
            QuanLyMenuCRUDViewModel MenuItem = await _context.QuanLyMenu.FirstOrDefaultAsync(m => m.Id == Id);
            List<QuanLyMenuCRUDViewModel> ListMenuItem = _iCommon.LoadQuanLyMenuByParentId(MenuItem.ParentId).ToList();
            var index = ordering - 1;
            ListMenuItem.Swap(index, index + 1);
            for (var i = 0; i < ListMenuItem.Count(); i++)
            {
                var a_MenuItem = ListMenuItem[i];
                var _QuanLyMenu = await _context.QuanLyMenu.FindAsync(a_MenuItem.Id);
                _context.Entry(_QuanLyMenu).Property(u => u.Ordering).CurrentValue = i + 1;
                await _context.SaveChangesAsync();

              
            }

            JsonResultViewModel _JsonResultViewModel = new();

            try
            {
                var _GetGridItem = GetGridItem();
                _JsonResultViewModel.AlertMessage = "lấy dữ liệu thành công";
                _JsonResultViewModel.IsSuccess = true;
                _JsonResultViewModel.ModelObject = _GetGridItem;
                return new JsonResult(_JsonResultViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public IActionResult GetDataTabelData()
        {
            JsonResultViewModel _JsonResultViewModel = new();
            try
            {
                var _GetGridItem = GetGridItem();
                _JsonResultViewModel.AlertMessage = "lấy dữ liệu thành công";
                _JsonResultViewModel.IsSuccess = true;
                _JsonResultViewModel.ModelObject = _GetGridItem;
                return new JsonResult(_JsonResultViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private List<QuanLyMenuCRUDViewModel> GetGridItem()
        {
            List<QuanLyMenuCRUDViewModel> QuanLyMenuCRUDViewModelList = new List<QuanLyMenuCRUDViewModel>();
            _iCommon.LoadQuanLyMenu(ref QuanLyMenuCRUDViewModelList, 0, 1);

            return QuanLyMenuCRUDViewModelList;
        }
        [HttpGet]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();
            QuanLyMenuCRUDViewModel vm = await _context.QuanLyMenu.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> GetViewConfig(string view)
        {
            if (view == null) return NotFound();
            ViewConfig _ViewConfig = new ViewConfig();
            return PartialView("_Details", _ViewConfig);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            QuanLyMenuCRUDViewModel vm = new QuanLyMenuCRUDViewModel();
            if (id > 0) vm = await _context.QuanLyMenu.Where(x => x.Id == id).SingleOrDefaultAsync();
            List<QuanLyMenuCRUDViewModel> QuanLyMenuCRUDViewModelList = new List<QuanLyMenuCRUDViewModel>();
            QuanLyMenuCRUDViewModelList.Add(new QuanLyMenuCRUDViewModel
            {
                Id = 1,
                ParentId = 1,
                Name = "Root",
                Label=""
            });

            if (id > 0)
            {
                _iCommon.LoadQuanLyMenu(ref QuanLyMenuCRUDViewModelList, 1, 1, id);
            }
            else
            {
                _iCommon.LoadQuanLyMenu(ref QuanLyMenuCRUDViewModelList, 1, 1);
            }
            ViewBag._LoadObjectView = new SelectList(_iCommon.LoadObjectView(), "Id", "Name");
            ViewBag._LoadQuanLyMenu = new SelectList(QuanLyMenuCRUDViewModelList, "Id", "Label");
            ViewBag._LoadTask = new SelectList(LoadTask(), "Id", "Name");
            ViewBag._LoadEnvironment = new SelectList(LoadEnvironment(), "Id", "Name");
            var FileContent = System.IO.File.ReadAllText(Path.Combine(_iCommon.GetContentRootPath(), "wwwroot/AdminLTE/plugins/fontawesome-free/css/all.css"));
            List<string> listIcon = new List<string>();
            const string pattern = @".(?<L>.+?):before";

            foreach (Match match in Regex.Matches(FileContent, pattern, RegexOptions.IgnoreCase))
            {
                listIcon.Add(match.Groups["L"].Value);

            }
            ViewBag.ListIcon = JsonConvert.SerializeObject(listIcon);
            return PartialView("_AddEdit", vm);
        }
        public List<object> LoadTask()
        {
            List<object> values = new List<object>();
            values.Add(new { Id="Index", Name = "Index" });
            values.Add(new { Id="AddEdit", Name = "AddEdit" });
            values.Add(new { Id="Report", Name = "Report" });
            return values;
        }
        public List<object> LoadEnvironment()
        {
            List<object> values = new List<object>();
            values.Add(new { Id="dev", Name = "Development" });
            values.Add(new { Id= "production", Name = "Production" });
            values.Add(new { Id= "testing", Name = "Testing" });
            return values;
        }
        [HttpPost]
        public async Task<JsonResult> AddEdit(QuanLyMenuCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    QuanLyMenu _QuanLyMenu = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _QuanLyMenu = await _context.QuanLyMenu.FindAsync(vm.Id);

                        vm.CreatedDate = _QuanLyMenu.CreatedDate;
                        vm.CreatedBy = _QuanLyMenu.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_QuanLyMenu).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "QuanLyMenu đã cập nhật thành công ID: " + _QuanLyMenu.Id;
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _QuanLyMenu = vm;
                        _QuanLyMenu.CreatedDate = DateTime.Now;
                        _QuanLyMenu.ModifiedDate = DateTime.Now;
                        _QuanLyMenu.CreatedBy = _UserName;
                        _QuanLyMenu.ModifiedBy = _UserName;
                        _context.Add(_QuanLyMenu);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "QuanLyMenu đã thêm thành công ID: " + _QuanLyMenu.Id;
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
                var _QuanLyMenu = await _context.QuanLyMenu.FindAsync(id);
                _QuanLyMenu.ModifiedDate = DateTime.Now;
                _QuanLyMenu.ModifiedBy = HttpContext.User.Identity.Name;
                _QuanLyMenu.Cancelled = true;

                _context.Update(_QuanLyMenu);
                await _context.SaveChangesAsync();
                return new JsonResult(_QuanLyMenu);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.QuanLyMenu.Any(e => e.Id == id);
            

        }
    }
    public static class Extensions
    {
        public static void Swap<T>(this List<T> list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
