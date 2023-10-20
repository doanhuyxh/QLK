using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.QuanLyNhomNguoiDungViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using static Uno.UI.FeatureConfiguration;
using Models.QuanLyMenuViewModel;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class QuanLyNhomNguoiDungController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IFunctional _functional;
        private readonly IConfiguration _configuration;
        public readonly string user;

        public QuanLyNhomNguoiDungController(ApplicationDbContext context, ICommon iCommon, IFunctional functional, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _functional = functional;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name??string.Empty;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "QuanLyNhomNguoiDung.QuanLyNhomNguoiDung.list"))
            {
                return View();
            }
            else if (_iCommon.CheckPermissionInProfile(user, "QuanLyNhomNguoiDung.QuanLyNhomNguoiDung.list_owner"))
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
                    || obj.TenNhom.ToLower().Contains(searchValue)
                    || obj.MoTa.ToLower().Contains(searchValue)
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
        private IQueryable<QuanLyNhomNguoiDungCRUDViewModel> GetGridItem()
        {
            if (_iCommon.CheckPermissionInProfile(user, "QuanLyNhomNguoiDung.QuanLyNhomNguoiDung.list_owner"))
            {
                try
                {
                    return (from _QuanLyNhomNguoiDung in _context.QuanLyNhomNguoiDung
                            where _QuanLyNhomNguoiDung.Cancelled == false && _QuanLyNhomNguoiDung.CreatedBy.Contains(user)

                            select new QuanLyNhomNguoiDungCRUDViewModel
                            {
                                Id = _QuanLyNhomNguoiDung.Id,
                                TenNhom = _QuanLyNhomNguoiDung.TenNhom,
                                MoTa = _QuanLyNhomNguoiDung.MoTa,
                                CreatedDate = _QuanLyNhomNguoiDung.CreatedDate,
                                ModifiedDate = _QuanLyNhomNguoiDung.ModifiedDate,
                                CreatedBy = _QuanLyNhomNguoiDung.CreatedBy,
                                ModifiedBy = _QuanLyNhomNguoiDung.ModifiedBy,

                            }).OrderByDescending(x => x.Id);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            try
            {
                return (from _QuanLyNhomNguoiDung in _context.QuanLyNhomNguoiDung
                        where _QuanLyNhomNguoiDung.Cancelled == false

                        select new QuanLyNhomNguoiDungCRUDViewModel
                        {
                            Id = _QuanLyNhomNguoiDung.Id,
                            TenNhom = _QuanLyNhomNguoiDung.TenNhom,
                            MoTa = _QuanLyNhomNguoiDung.MoTa,
                            CreatedDate = _QuanLyNhomNguoiDung.CreatedDate,
                            ModifiedDate = _QuanLyNhomNguoiDung.ModifiedDate,
                            CreatedBy = _QuanLyNhomNguoiDung.CreatedBy,
                            ModifiedBy = _QuanLyNhomNguoiDung.ModifiedBy,

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
            QuanLyNhomNguoiDungCRUDViewModel vm = await _context.QuanLyNhomNguoiDung.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            QuanLyNhomNguoiDungCRUDViewModel vm = new QuanLyNhomNguoiDungCRUDViewModel();
            if (id > 0) vm = await _context.QuanLyNhomNguoiDung.Where(x => x.Id == id).SingleOrDefaultAsync();
            List< KeyValuePair<string, List< KeyValuePair<string, List<KeyValuePair<string, Access>>>>>> ViewSectionAccessList = new List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, Access>>>>>>();
            List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, Access>>>>>> DyPermistions =new List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, Access>>>>>>();
            if (id > 0)
            {
                var Permistions = vm.Permistions;
                DyPermistions = JsonConvert.DeserializeObject<List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, Access>>>>>>>(Permistions);
            }
            else
            {

            }
            string[] views = Directory.GetDirectories(string.Concat(_iCommon.GetContentRootPath(), "wwwroot//Views"), "*", SearchOption.TopDirectoryOnly);

            foreach (string view in views)
            {
                XmlDocument doc = new XmlDocument();
                var accessXmlFile = string.Concat(view, "/access.xml");
                DirectoryInfo dir_info = new DirectoryInfo(view);
                string ViewName = dir_info.Name;
                if (System.IO.File.Exists(accessXmlFile))
                {
                    doc.Load(accessXmlFile);
                    List<KeyValuePair<string, List<KeyValuePair<string, Access>>>> SectionAccessList = new List<KeyValuePair<string, List<KeyValuePair<string, Access>>>>();
                    foreach (XmlNode section in doc.SelectNodes("/access/section"))
                    {
                        string SectionName = section.Attributes["name"].Value;
                        List<KeyValuePair<string, Access>> AccessList1 = new List<KeyValuePair<string,Access>>();
                        foreach (XmlNode action in section.ChildNodes)
                        {
                            var Name = action.Attributes["name"].Value;
                            var CurrentValue = false;
                            try
                            {
                                CurrentValue = DyPermistions.FirstOrDefault(item => item.Key == ViewName).Value.FirstOrDefault(item => item.Key == SectionName).Value.FirstOrDefault(item => item.Key == Name).Value.CurrentValue;
                            }
                            catch(Exception ex )
                            {

                            }
                            AccessList1.Add(new KeyValuePair<string, Access>(Name, new Access
                            {
                                Name = Name,
                                Title = action.Attributes["title"].Value,
                                Description = action.Attributes["description"].Value,
                                CurrentValue = CurrentValue
                            }));
                        }
                        SectionAccessList.Add(new KeyValuePair<string, List<KeyValuePair<string, Access>>>(SectionName, AccessList1));

                    }
                    ViewSectionAccessList.Add((new KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, Access>>>>>(ViewName, SectionAccessList)));
                }
            }

            vm.ViewSectionAccessList = ViewSectionAccessList;
            ViewBag.JsonViewSectionAccessList = JsonConvert.SerializeObject(ViewSectionAccessList);
            
            return PartialView("_AddEdit", vm);
        }
        //[INSERT_LOAD]

        [HttpPost]
        public async Task<JsonResult> AddEdit(QuanLyNhomNguoiDungCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    QuanLyNhomNguoiDung _QuanLyNhomNguoiDung = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _QuanLyNhomNguoiDung = await _context.QuanLyNhomNguoiDung.FindAsync(vm.Id);
                        vm.CreatedDate = _QuanLyNhomNguoiDung.CreatedDate;
                        vm.CreatedBy = _QuanLyNhomNguoiDung.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_QuanLyNhomNguoiDung).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();
                        
                        var _AlertMessage = "Đã cập nhật thành công nhóm: "+vm.TenNhom;
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _QuanLyNhomNguoiDung = vm;
                        

                        _QuanLyNhomNguoiDung.CreatedDate = DateTime.Now;
                        _QuanLyNhomNguoiDung.ModifiedDate = DateTime.Now;
                        _QuanLyNhomNguoiDung.CreatedBy = _UserName;
                        _QuanLyNhomNguoiDung.ModifiedBy = _UserName;
                        _context.Add(_QuanLyNhomNguoiDung);
                        await _context.SaveChangesAsync();
                        
                        var _AlertMessage = "Đã thêm thành công nhóm: " + _QuanLyNhomNguoiDung.TenNhom;
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

            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "QuanLyNhomNguoiDung.QuanLyNhomNguoiDung.delete"))
            {
                try
                {
                    var _QuanLyNhomNguoiDung = await _context.QuanLyNhomNguoiDung.FindAsync(id);
                    _QuanLyNhomNguoiDung.ModifiedDate = DateTime.Now;
                    _QuanLyNhomNguoiDung.ModifiedBy = HttpContext.User.Identity.Name;
                    _QuanLyNhomNguoiDung.Cancelled = true;

                    _context.Update(_QuanLyNhomNguoiDung);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _QuanLyNhomNguoiDung;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Xóa thất bại";
                    _JsonResultViewModel.IsSuccess = false;
                    _JsonResultViewModel.ModelObject = null;
                }
            }
            else if (_iCommon.CheckPermissionInProfile(user, "QuanLyNhomNguoiDung.QuanLyNhomNguoiDung.delete_owner"))
            {
                try
                {
                    var _QuanLyNhomNguoiDung = await _context.QuanLyNhomNguoiDung.FindAsync(id);
                    _QuanLyNhomNguoiDung.ModifiedDate = DateTime.Now;
                    _QuanLyNhomNguoiDung.ModifiedBy = HttpContext.User.Identity.Name;
                    _QuanLyNhomNguoiDung.Cancelled = true;

                    _context.Update(_QuanLyNhomNguoiDung);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _QuanLyNhomNguoiDung;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Xóa thất bại";
                    _JsonResultViewModel.IsSuccess = false;
                    _JsonResultViewModel.ModelObject = null;
                }
            }


            _JsonResultViewModel.AlertMessage = "Bạn không có quyền xóa";
            _JsonResultViewModel.IsSuccess = true;
            _JsonResultViewModel.ModelObject = null;

            return new JsonResult(_JsonResultViewModel);
        }

        private bool IsExists(long id)
        {
            return _context.QuanLyNhomNguoiDung.Any(e => e.Id == id);
        }
    }
}
