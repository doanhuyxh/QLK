using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.XayDungCongThucViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
using AMS.Helpers;
using AMS.Models.ListNguyenLieuInCongThucViewModel;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using Rotativa.AspNetCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class XayDungCongThucController : Controller
    {
        private readonly IWebHostEnvironment _iwebhostenvironment;
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string user;
        private readonly IConfiguration _configuration;

        public XayDungCongThucController(IConfiguration configuration, IWebHostEnvironment iwebhostenvironment, ApplicationDbContext context, ICommon iCommon, UserManager<ApplicationUser> userManager, IHttpContextAccessor accessor)
        {
            this._iwebhostenvironment = iwebhostenvironment;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            _context = context;
            _iCommon = iCommon;
            _userManager = userManager;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name ?? string.Empty;
        }



        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "XayDungCongThuc.XayDungCongThuc.list"))
            {
                return View();
            }
            else if (_iCommon.CheckPermissionInProfile(user, "XayDungCongThuc.XayDungCongThuc.list_owner"))
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
                    || obj.Name.ToLower().Contains(searchValue)
                    || obj.Description.ToLower().Contains(searchValue)
                    || obj.MaSP.ToLower().Contains(searchValue)
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

        private IQueryable<XayDungCongThucCRUDViewModels> GetGridItem()
        {
            if (_iCommon.CheckPermissionInProfile(user, "XayDungCongThuc.XayDungCongThuc.list_owner"))
            {
                try
                {
                    return (from _DanhSachXayDungCongThuc in _context.XayDungCongThuc
                            where _DanhSachXayDungCongThuc.Cancelled == false && _DanhSachXayDungCongThuc.CreatedBy.Contains(user)

                            select new XayDungCongThucCRUDViewModels
                            {
                                Id = _DanhSachXayDungCongThuc.Id,
                                Name = _DanhSachXayDungCongThuc.Name,
                                Description = _DanhSachXayDungCongThuc.Description,
                                MaSP = _DanhSachXayDungCongThuc.MaSP,
                                CreatedDate = _DanhSachXayDungCongThuc.CreatedDate,
                                ModifiedDate = _DanhSachXayDungCongThuc.ModifiedDate,
                                CreatedBy = _DanhSachXayDungCongThuc.CreatedBy,
                                ModifiedBy = _DanhSachXayDungCongThuc.ModifiedBy,
                            }).OrderByDescending(x => x.Id);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            try
            {
                return (from _DanhSachXayDungCongThuc in _context.XayDungCongThuc
                        where _DanhSachXayDungCongThuc.Cancelled == false

                        select new XayDungCongThucCRUDViewModels
                        {
                            Id = _DanhSachXayDungCongThuc.Id,
                            Name = _DanhSachXayDungCongThuc.Name,
                            Description = _DanhSachXayDungCongThuc.Description,
                            MaSP = _DanhSachXayDungCongThuc.MaSP,
                            CreatedDate = _DanhSachXayDungCongThuc.CreatedDate,
                            ModifiedDate = _DanhSachXayDungCongThuc.ModifiedDate,
                            CreatedBy = _DanhSachXayDungCongThuc.CreatedBy,
                            ModifiedBy = _DanhSachXayDungCongThuc.ModifiedBy,
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
            XayDungCongThucCRUDViewModels vm = await _context.XayDungCongThuc.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            XayDungCongThucCRUDViewModels vm = new XayDungCongThucCRUDViewModels();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id > 0)
            {
                vm = await _context.XayDungCongThuc.Where(x => x.Id == id).SingleOrDefaultAsync();
                ViewBag._LoadNguyenLieu = JsonConvert.SerializeObject(GetNguyenLieuByXayDungCongThucId(id));

                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "XayDungCongThuc.XayDungCongThuc.edit"))
                {
                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "XayDungCongThuc.XayDungCongThuc.edit_owner"))
                {
                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                ViewBag._LoadNguyenLieu = JsonConvert.SerializeObject(Array.Empty<ListNguyenLieuInCongThucViewModel>());
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "XayDungCongThuc.XayDungCongThuc.create"))
                {
                    return PartialView("_AddEdit", vm);
                }
            }

            return View("_AccessDeniedAddEdit");
        }

        [HttpPost]
        public async Task<JsonResult> AddEdit(XayDungCongThucCRUDViewModels vm)
        {
            try
            {
                JsonResultViewModel _JsonResultViewModel = new();

                if (ModelState.IsValid)
                {
                    XayDungCongThuc _DanhSachXayDungCongThuc = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


                    if (vm.Id > 0)
                    {
                        _DanhSachXayDungCongThuc = await _context.XayDungCongThuc.FindAsync(vm.Id);

                        await _context.SaveChangesAsync();
                        vm.CreatedDate = _DanhSachXayDungCongThuc.CreatedDate;
                        vm.CreatedBy = _DanhSachXayDungCongThuc.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_DanhSachXayDungCongThuc).CurrentValues.SetValues(vm);

                        foreach (var item in vm.ListNguyenLieuInCongThuc)
                        {
                            if (item.Id > 0)
                            {
                                var nl = await _context.ListNguyenLieuInCongThuc.FindAsync(item.Id);
                                _context.Entry(nl).CurrentValues.SetValues(item);
                                await _context.SaveChangesAsync();

                            }
                            else
                            {
                                ListNguyenLieuInCongThuc sp = item;
                                sp.SanPhamId = _DanhSachXayDungCongThuc.Id;
                                sp.CreatedDate = DateTime.Now;
                                sp.ModifiedDate = DateTime.Now;
                                sp.CreatedBy = _UserName;
                                sp.ModifiedBy = _UserName;
                                _context.Add(sp);
                                await _context.SaveChangesAsync();
                            }
                        }
                        _JsonResultViewModel.ModelObject = vm;
                        _JsonResultViewModel.AlertMessage = "Công thức: " + vm.Name + " đã cập nhật thành công";
                        _JsonResultViewModel.IsSuccess = true;
                        return new JsonResult(_JsonResultViewModel);
                       
                    }
                    else
                    {
                        _DanhSachXayDungCongThuc = vm;
                        _DanhSachXayDungCongThuc.CreatedDate = DateTime.Now;
                        _DanhSachXayDungCongThuc.ModifiedDate = DateTime.Now;
                        _DanhSachXayDungCongThuc.CreatedBy = _UserName;
                        _DanhSachXayDungCongThuc.ModifiedBy = _UserName;
                        _context.Add(_DanhSachXayDungCongThuc);
                        await _context.SaveChangesAsync();

                        foreach (var item in vm.ListNguyenLieuInCongThuc)
                        {
                            ListNguyenLieuInCongThuc sp = item;
                            sp.SanPhamId = _DanhSachXayDungCongThuc.Id;
                            sp.CreatedDate = DateTime.Now;
                            sp.ModifiedDate = DateTime.Now;
                            sp.CreatedBy = _UserName;
                            sp.ModifiedBy = _UserName;
                            _context.Add(sp);
                            await _context.SaveChangesAsync();

                        }
                        _JsonResultViewModel.ModelObject = vm;
                        _JsonResultViewModel.AlertMessage = "Công thức: " + vm.Name + " đã thêm thành công";
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


            JsonResultViewModel _JsonResultViewModel = new();

            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "XayDungCongThuc.XayDungCongThuc.delete"))
            {
                try
                {
                    var _DanhSachXayDungCongThuc = await _context.XayDungCongThuc.FindAsync(id);
                    _DanhSachXayDungCongThuc.ModifiedDate = DateTime.Now;
                    _DanhSachXayDungCongThuc.ModifiedBy = HttpContext.User.Identity.Name;
                    _DanhSachXayDungCongThuc.Cancelled = true;

                    _context.Update(_DanhSachXayDungCongThuc);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _DanhSachXayDungCongThuc;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Xóa thất bại";
                    _JsonResultViewModel.IsSuccess = false;
                    _JsonResultViewModel.ModelObject = null;
                    return new JsonResult(_JsonResultViewModel);
                }
            }
            else if (_iCommon.CheckPermissionInProfile(user, "XayDungCongThuc.XayDungCongThuc.delete_owner"))
            {
                try
                {
                    var _DanhSachXayDungCongThuc = await _context.XayDungCongThuc.FindAsync(id);
                    _DanhSachXayDungCongThuc.ModifiedDate = DateTime.Now;
                    _DanhSachXayDungCongThuc.ModifiedBy = HttpContext.User.Identity.Name;
                    _DanhSachXayDungCongThuc.Cancelled = true;

                    _context.Update(_DanhSachXayDungCongThuc);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _DanhSachXayDungCongThuc;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Xóa thất bại";
                    _JsonResultViewModel.IsSuccess = false;
                    _JsonResultViewModel.ModelObject = null;
                    return new JsonResult(_JsonResultViewModel);
                }
            }


            _JsonResultViewModel.AlertMessage = "Bạn không có quyền xóa";
            _JsonResultViewModel.IsSuccess = true;
            _JsonResultViewModel.ModelObject = null;

            return new JsonResult(_JsonResultViewModel);


        }

        private bool IsExists(long id)
        {
            return _context.XayDungCongThuc.Any(e => e.Id == id);
        }

        [HttpGet]
        public IActionResult GetXayDungCongThucAPI()
        {
            return Ok(GetGridItem());
        }

        public List<ListNguyenLieuInCongThucViewModel> GetNguyenLieuByXayDungCongThucId(int id)
        {

            var result = from _nl in _context.ListNguyenLieuInCongThuc
                         where _nl.SanPhamId == id && _nl.Cancelled == false
                         select new ListNguyenLieuInCongThucViewModel
                         {
                             Id = _nl.Id,
                             MieuTa = _nl.MieuTa,
                             NguyenLieuId = _nl.NguyenLieuId,
                             SanPhamId = _nl.SanPhamId,
                             SoMetChi = _nl.SoMetChi,
                             Cancelled = _nl.Cancelled,
                             CachSuDung = _nl.CachSuDung,
                             ListSizeMau = _nl.ListSizeMau,
                             DonVi = _nl.DonVi,
                             SoLuong = _nl.SoLuong,
                             DinhMuc = _nl.DinhMuc,
                             NhuCau = _nl.NhuCau,
                             SoLo = _nl.SoLo,
                             NgayVeVIT = _nl.NgayVeVIT,
                             ThucNhan = _nl.ThucNhan,
                         };
            return result.ToList();
        }

        public List<XayDungCongThucCRUDViewModels> getDanhMucLoaiThuocCRUDViewModelList()
        {

            List<XayDungCongThucCRUDViewModels> NhapKhoCRUDViewModellList = (
                from tblObj in _context.XayDungCongThuc.OrderBy(x => x.Id)
                where tblObj.Cancelled == false
                select new XayDungCongThucCRUDViewModels
                {
                    Id = tblObj.Id,
                    Name = tblObj.Name,
                    Description = tblObj.Description,
                    MaSP = tblObj.MaSP,
                    CreatedDate = tblObj.CreatedDate,
                    ModifiedDate = tblObj.ModifiedDate,
                    CreatedBy = tblObj.CreatedBy,
                    ModifiedBy = tblObj.ModifiedBy,

                }).ToList();
            return NhapKhoCRUDViewModellList;
        }
    }

}
