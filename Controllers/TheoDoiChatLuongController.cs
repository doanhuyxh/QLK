using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.TheoDoiChatLuongViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
using AMS.Models.NguyenLieuViewModel;
using AMS.Models.NguyenLieuKeHoachCRUDViewModel;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class TheoDoiChatLuongController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public TheoDoiChatLuongController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        //[Authorize(Roles = AMS.Pages.MainMenu.TheoDoiChatLuong.RoleName)]
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
                    || obj.TenNguyenLieuId.ToString().ToLower().Contains(searchValue)
                    || obj.GhiChuVeChatLuong.ToLower().Contains(searchValue)
                    || (
                        obj.ChatLuongDrop.ToString().ToLower().Contains(searchValue) &&
                        (obj.ChatLuongDrop == 1 && "Tốt" == searchValue ||
                        obj.ChatLuongDrop == 2 && "Khá" == searchValue ||
                        obj.ChatLuongDrop == 3 && "Đạt" == searchValue ||
                        obj.ChatLuongDrop == 4 && "Không đạt" == searchValue)
                    )
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
        private IQueryable<TheoDoiChatLuongCRUDViewModel> GetGridItem()
        {
            try
            {
                return (from _TheoDoiChatLuong in _context.TheoDoiChatLuong
                        join Obj in _context.NguyenLieu on _TheoDoiChatLuong.TenNguyenLieuId equals Obj.Id
                        where _TheoDoiChatLuong.Cancelled == false

                        select new TheoDoiChatLuongCRUDViewModel
                        {
                            Id = _TheoDoiChatLuong.Id,
                            TenNguyenLieuId = _TheoDoiChatLuong.TenNguyenLieuId,
                            GhiChuVeChatLuong = _TheoDoiChatLuong.GhiChuVeChatLuong,
                            ChatLuongDrop = _TheoDoiChatLuong.ChatLuongDrop,
                            CreatedDate = _TheoDoiChatLuong.CreatedDate,
                            ModifiedDate = _TheoDoiChatLuong.ModifiedDate,
                            CreatedBy = _TheoDoiChatLuong.CreatedBy,
                            ModifiedBy = _TheoDoiChatLuong.ModifiedBy,
                            DisplayName = Obj.TenNguyenLieu


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
            TheoDoiChatLuongCRUDViewModel vm = await _context.TheoDoiChatLuong.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            TheoDoiChatLuongCRUDViewModel vm = new TheoDoiChatLuongCRUDViewModel();
            if (id > 0)
            {
                await _context.TheoDoiChatLuong.Where(x => x.Id == id).SingleOrDefaultAsync();
                ViewBag._LoadThanhPhamSanXuat = JsonConvert.SerializeObject(LoadNguyenLieu());
            }
            else
            {
                ViewBag._LoadThanhPhamSanXuat = JsonConvert.SerializeObject(Array.Empty<TheoDoiChatLuongCRUDViewModel>());
            }

            //[INSERT_OBJECTSUBVIEWFIELDLIST]
            return PartialView("_AddEdit", vm);
        }
        //[INSERT_LOAD]
        public IQueryable<TheoDoiChatLuongCRUDViewModel> LoadNguyenLieu()
        {
            return (from tblObj in _context.TheoDoiChatLuong.Where(x => x.Cancelled == false).OrderBy(x => x.Id)
                    select new TheoDoiChatLuongCRUDViewModel
                    {
                        Id = tblObj.Id,
                        TenNguyenLieuId = tblObj.TenNguyenLieuId,
                        GhiChuVeChatLuong = tblObj.GhiChuVeChatLuong,
                        ChatLuongDrop = tblObj.ChatLuongDrop,
                        CreatedDate = tblObj.CreatedDate,
                        ModifiedDate = tblObj.ModifiedDate,
                    }
            );
        }

        [HttpPost]
        public async Task<JsonResult> AddEdit(TheoDoiChatLuongCRUDViewModel vm)
        {
            try
            {
                JsonResultViewModel _JsonResultViewModel = new();

                if (ModelState.IsValid)
                {
                    TheoDoiChatLuong _TheoDoiChatLuong = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _TheoDoiChatLuong = await _context.TheoDoiChatLuong.FindAsync(vm.Id);

                        vm.CreatedDate = _TheoDoiChatLuong.CreatedDate;
                        vm.CreatedBy = _TheoDoiChatLuong.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_TheoDoiChatLuong).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        var _AlertMessage = "Theo dõi chất lượng đã cập nhật thành công";
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _TheoDoiChatLuong = vm;

                        _TheoDoiChatLuong.CreatedDate = DateTime.Now;
                        _TheoDoiChatLuong.ModifiedDate = DateTime.Now;
                        _TheoDoiChatLuong.CreatedBy = _UserName;
                        _TheoDoiChatLuong.ModifiedBy = _UserName;
                        _context.Add(_TheoDoiChatLuong);
                        await _context.SaveChangesAsync();

                        _JsonResultViewModel.ModelObject = vm;
                        _JsonResultViewModel.AlertMessage = "Theo dõi chất lượng đã thêm thành công ";
                        _JsonResultViewModel.IsSuccess = true;
                        return new JsonResult(_JsonResultViewModel);
                    }
                }
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
                _JsonResultViewModel.ModelObject = null;
                _JsonResultViewModel.AlertMessage = messages;
                _JsonResultViewModel.IsSuccess = false;
                return new JsonResult(_JsonResultViewModel);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                JsonResultViewModel _JsonResultViewModel = new();
                _JsonResultViewModel.ModelObject = null;
                _JsonResultViewModel.AlertMessage = "Có lỗi khi thêm dữ liệu vào cơ sở dữ liệu";
                _JsonResultViewModel.IsSuccess = false;
                return new JsonResult(_JsonResultViewModel);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var _TheoDoiChatLuong = await _context.TheoDoiChatLuong.FindAsync(id);
                _TheoDoiChatLuong.ModifiedDate = DateTime.Now;
                _TheoDoiChatLuong.ModifiedBy = HttpContext.User.Identity.Name;
                _TheoDoiChatLuong.Cancelled = true;

                _context.Update(_TheoDoiChatLuong);
                await _context.SaveChangesAsync();
                return new JsonResult(_TheoDoiChatLuong);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public IActionResult GetApiDanhGia()
        {
            var a = from _nl in _context.TheoDoiChatLuong
                    join _name in _context.NguyenLieu on _nl.TenNguyenLieuId equals _name.Id
                    where _nl.Cancelled == false
                    select new
                    {
                        ID = _nl.Id,
                        TenNguyenLieuId = _nl.TenNguyenLieuId,
                        GhiChuVeChatLuong = _nl.GhiChuVeChatLuong,
                        ChatLuongDrop = _nl.ChatLuongDrop,
                        CreatedDate = _nl.CreatedDate,
                        TenNguyenLieu = _name.TenNguyenLieu
                    };
            return Ok(a.ToList());

        }
        private bool IsExists(long id)
        {
            return _context.TheoDoiChatLuong.Any(e => e.Id == id);
        }
    }
}
