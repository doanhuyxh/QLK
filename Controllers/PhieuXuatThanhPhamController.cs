using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.PhieuXuatThanhPhamViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
using AMS.Models.ListNhapThanhPhamViewModel;
using AMS.Models.ListXuatThanhPhamViewModel;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class PhieuXuatThanhPhamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;
        public PhieuXuatThanhPhamController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name ?? string.Empty;
        }

        //[Authorize(Roles = AMS.Pages.MainMenu.PhieuXuatThanhPham.RoleName)]
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
                    || obj.MaPhieuXuat.ToLower().Contains(searchValue)
                    || obj.NgayDuKienXuat.ToString().ToLower().Contains(searchValue)
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
        private IQueryable<PhieuXuatThanhPhamCRUDViewModel> GetGridItem()
        {
            try
            {
                return (from _PhieuXuatThanhPham in _context.PhieuXuatThanhPham
                        where _PhieuXuatThanhPham.Cancelled == false

                        select new PhieuXuatThanhPhamCRUDViewModel
                        {
                            Id = _PhieuXuatThanhPham.Id,
                            MaPhieuXuat = _PhieuXuatThanhPham.MaPhieuXuat,
                            NgayDuKienXuat = _PhieuXuatThanhPham.NgayDuKienXuat,
                            CreatedDate = _PhieuXuatThanhPham.CreatedDate,
                            ModifiedDate = _PhieuXuatThanhPham.ModifiedDate,
                            CreatedBy = _PhieuXuatThanhPham.CreatedBy,
                            ModifiedBy = _PhieuXuatThanhPham.ModifiedBy,

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
            PhieuXuatThanhPhamCRUDViewModel vm = await _context.PhieuXuatThanhPham.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            
            PhieuXuatThanhPhamCRUDViewModel vm = new PhieuXuatThanhPhamCRUDViewModel();

            if (id > 0)
            {
                vm = await _context.PhieuXuatThanhPham.Where(x => x.Id == id).SingleOrDefaultAsync();
                ViewBag.PhieuXuatThanhPham = JsonConvert.SerializeObject(GetThanhPhamNhapKhoByNhapThanhPhamId(id));
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "PhieuXuatThanhPham.PhieuXuatThanhPham.edit"))
                {
                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "PhieuXuatThanhPham.PhieuXuatThanhPham.edit_owner"))
                {
                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "PhieuXuatThanhPham.PhieuXuatThanhPham.create"))
                {
                    ViewBag.PhieuXuatThanhPham = JsonConvert.SerializeObject(Array.Empty<PhieuXuatThanhPhamCRUDViewModel>());
                    return PartialView("_AddEdit", vm);
                }
            }
            //[INSERT_OBJECTSUBVIEWFIELDLIST]
            return View("_AccessDeniedAddEdit");
        }
        //[INSERT_LOAD]
        private List<ListXuatThanhPham> GetThanhPhamNhapKhoByNhapThanhPhamId(int id)
        {
            var rs = from _tp in _context.ListXuatThanhPham
                     where _tp.IDPhieu == id && _tp.Cancelled == false
                     select new ListXuatThanhPham
                     {
                         ID = _tp.ID,
                         IDPhieu = _tp.IDPhieu,
                         IDThanhPham = _tp.IDThanhPham,
                         PO = _tp.PO,
                         Cancelled = _tp.Cancelled,
                         Mau = _tp.Mau,
                         Size = _tp.Size,
                         SoLuong = _tp.SoLuong,
                         DonGia = _tp.DonGia,
                         NgayNhap = _tp.NgayNhap,
                         KhachHang = _tp.KhachHang,
                         MaHang = _tp.MaHang,

                     };
            return rs.ToList();
        }
        [HttpPost]
        public async Task<JsonResult> AddEdit(PhieuXuatThanhPhamCRUDViewModel vm)
        {
            try
            {
                JsonResultViewModel _JsonResultViewModel = new();

                if (ModelState.IsValid)
                {
                    PhieuXuatThanhPham _PhieuXuatThanhPham = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _PhieuXuatThanhPham = await _context.PhieuXuatThanhPham.FindAsync(vm.Id);


                        vm.CreatedDate = _PhieuXuatThanhPham.CreatedDate;
                        vm.CreatedBy = _PhieuXuatThanhPham.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_PhieuXuatThanhPham).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        foreach (var item in vm.ThanhPhamNhapKhoList)
                        {
                            ListXuatThanhPham tp = new ListXuatThanhPham();
                            if (item.ID > 0)
                            {
                                tp = _context.ListXuatThanhPham.FirstOrDefault(x => x.ID == item.ID);
                                tp = item;

                                tp.Cancelled = item.Cancelled;
                                tp.SoLuong = item.SoLuong;
                                tp.DonGia = item.DonGia;
                                tp.Mau = item.Mau;
                                tp.Size = item.Size;
                                tp.PO = item.PO;
                                tp.IDPhieu = item.IDPhieu;
                                tp.ModifiedDate = DateTime.Now;
                                tp.ModifiedBy = _UserName;
                                _context.Update(tp);
                                await _context.SaveChangesAsync();

                                QuanLyThanhPham tp2 = new QuanLyThanhPham();
                                tp2 = _context.QuanLyThanhPham.FirstOrDefault(i => i.Id == tp.IDThanhPham);
                                tp2.SoLuong = tp2.SoLuong - tp.SoLuong;
                                _context.Update(tp2);
                                await _context.SaveChangesAsync();

                            }
                            else
                            {
                                tp = item;

                                tp.ID = item.ID;
                                tp.IDPhieu = _PhieuXuatThanhPham.Id;
                                tp.SoLuong = item.SoLuong;
                                tp.DonGia = item.DonGia;
                                tp.Cancelled = item.Cancelled;
                                tp.IDThanhPham = item.IDThanhPham;
                                tp.PO = item.PO;
                                tp.Size = item.Size;
                                tp.Mau = item.Mau;
                                tp.CreatedBy = _UserName;
                                tp.ModifiedBy = _UserName;
                                tp.CreatedDate = DateTime.Now;
                                tp.ModifiedDate = DateTime.Now;
                                await _context.AddAsync(tp);
                                await _context.SaveChangesAsync();

                                QuanLyThanhPham tp2 = new QuanLyThanhPham();
                                tp2 = _context.QuanLyThanhPham.FirstOrDefault(i => i.Id == tp.IDThanhPham);
                                tp2.SoLuong = tp2.SoLuong - tp.SoLuong;

                                _context.Update(tp2);
                                await _context.SaveChangesAsync();
                            }

                        }
                        var _AlertMessage = "Đã thêm phiếu thành công";
                        return new JsonResult(_AlertMessage);

                        
                    }
                    else
                    {
                        _PhieuXuatThanhPham = vm;


                        _PhieuXuatThanhPham.CreatedDate = DateTime.Now;
                        _PhieuXuatThanhPham.ModifiedDate = DateTime.Now;
                        _PhieuXuatThanhPham.CreatedBy = _UserName;
                        _PhieuXuatThanhPham.ModifiedBy = _UserName;
                        _context.Add(_PhieuXuatThanhPham);
                        await _context.SaveChangesAsync();

                        foreach (var item in vm.ThanhPhamNhapKhoList)
                        {
                            ListXuatThanhPham tp = new ListXuatThanhPham();
                            tp = item;

                            tp.ID = item.ID;
                            tp.IDPhieu = _PhieuXuatThanhPham.Id;
                            tp.SoLuong = item.SoLuong;
                            tp.DonGia = item.DonGia;
                            tp.Cancelled = item.Cancelled;
                            tp.IDThanhPham = item.IDThanhPham;
                            tp.PO = item.PO;
                            tp.Size = item.Size;
                            tp.Mau = item.Mau;
                            tp.CreatedBy = _UserName;
                            tp.ModifiedBy = _UserName;
                            tp.CreatedDate = DateTime.Now;
                            tp.ModifiedDate = DateTime.Now;
                            await _context.AddAsync(tp);
                            await _context.SaveChangesAsync();

                            QuanLyThanhPham tp2 = new QuanLyThanhPham();
                            tp2 = _context.QuanLyThanhPham.FirstOrDefault(i => i.Id == tp.IDThanhPham);
                            tp2.SoLuong -= tp.SoLuong;
                            _context.Update(tp2);
                            await _context.SaveChangesAsync();
                        }
                        _JsonResultViewModel.ModelObject = vm;
                        _JsonResultViewModel.AlertMessage = "Đã thêm vào thành công, dữ liệu nguyên liệu đã được cập nhập";
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
                var _PhieuXuatThanhPham = await _context.PhieuXuatThanhPham.FindAsync(id);
                _PhieuXuatThanhPham.ModifiedDate = DateTime.Now;
                _PhieuXuatThanhPham.ModifiedBy = HttpContext.User.Identity.Name;
                _PhieuXuatThanhPham.Cancelled = true;

                _context.Update(_PhieuXuatThanhPham);
                await _context.SaveChangesAsync();
                return new JsonResult(_PhieuXuatThanhPham);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private bool IsExists(long id)
        {
            return _context.PhieuXuatThanhPham.Any(e => e.Id == id);
        }
        [HttpGet]
        public IActionResult GetAllMaPhieuThanhPhamByMa(string ma)
        {
            var rs = (from _ma in _context.PhieuXuatThanhPham
                      where _ma.Cancelled == false && _ma.MaPhieuXuat.StartsWith(ma)
                      select new
                      {
                          MaNhap = _ma.MaPhieuXuat
                      }
                      ).ToList();
            return Ok(rs);
        }
        public List<DtaPrint3> LoadPhieuNhapThanhPhamByNhapKhoId2(int IDPhieu)
        {
            return (from tblObj in _context.ListXuatThanhPham.Where(x => x.IDPhieu == IDPhieu && x.Cancelled == false).OrderBy(x => x.ID)
                    select new DtaPrint3
                    {
                        TenPhieu = _context.PhieuNhapThanhPham.FirstOrDefault(i => i.Id == tblObj.IDPhieu).MaNhap ?? "",
                        TenThanhPham = _context.QuanLyThanhPham.FirstOrDefault(t => t.Id == tblObj.IDThanhPham).TenThanhPham ?? "",
                        PO = tblObj.PO,
                        Mau = tblObj.Mau,
                        Size = tblObj.Size,
                        DonGia = tblObj.DonGia,
                        SoLuong = tblObj.SoLuong,
                       
                    }
            ).ToList();
        }

        public IActionResult Print(int id)
        {
            ViewBag.Load = LoadPhieuNhapThanhPhamByNhapKhoId2(id);
            return PartialView("_Print");
        }
        [HttpGet]
        public IActionResult GetListNguyenLieuNhapKhoLyThuyetKhiChuaNhapByID(int nhakholythuyet)
        {
            var a = (from _nl in _context.ListXuatThanhPham
                     join _ten in _context.PhieuXuatThanhPham on _nl.IDPhieu equals _ten.Id
                     where _nl.IDPhieu == nhakholythuyet
                     select new ListXuatThanhPhamCRUDViewModel
                     {
                         ID = _nl.ID,
                         IDPhieu = _nl.IDPhieu,
                         IDThanhPham = _nl.IDThanhPham,
                         PO = _nl.PO,
                         Mau = _nl.Mau,
                         Size = _nl.Size,
                         SoLuong = _nl.SoLuong,
                         DonGia = _nl.DonGia,
                         Cancelled = _nl.Cancelled,
                         NgayNhap = _nl.NgayNhap,
                         KhachHang = _nl.KhachHang,
                         MaHang = _nl.MaHang,
                         TenNguyenLieuByPhieuNhap = _ten.MaPhieuXuat,
                     }
                ).ToList();
            return Ok(a);
        }
        [HttpGet]
        public IActionResult GetListNhapKhoLyThuyetKhiChuaNhap()
        {
            var a = (from _nl in _context.PhieuXuatThanhPham
                     where _nl.Cancelled == false
                     select new PhieuXuatThanhPham
                     {
                         Id = _nl.Id,
                         MaPhieuXuat = _nl.MaPhieuXuat,
                         NgayDuKienXuat = _nl.NgayDuKienXuat,
                     }
                ).ToList();
            return Ok(a);
        }
    }
}
