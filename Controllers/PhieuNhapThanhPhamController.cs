using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.PhieuNhapThanhPhamViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
using AMS.Models.NhapThanhPhamViewModel;
using AMS.Models.NguyenLieuNhapKhoLyThuyetViewModel;
using AMS.Models.ListNhapThanhPhamViewModel;
using AMS.Models.NguyenLieuViewModel;
using AMS.Models.QuanLyThanhPhamViewModel;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class PhieuNhapThanhPhamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;

        public PhieuNhapThanhPhamController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name ?? string.Empty;


        }

        //[Authorize(Roles = AMS.Pages.MainMenu.PhieuNhapThanhPham.RoleName)]
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
                    || obj.MaNhap.ToLower().Contains(searchValue)
                    || obj.CreatedDate.ToString().ToLower().Contains(searchValue)
                    || obj.NgayDuKienNhap.ToString().ToLower().Contains(searchValue)
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
        private IQueryable<PhieuNhapThanhPhamCRUDViewModel> GetGridItem()
        {
            try
            {
                return (from _PhieuNhapThanhPham in _context.PhieuNhapThanhPham
                        where _PhieuNhapThanhPham.Cancelled == false

                        select new PhieuNhapThanhPhamCRUDViewModel
                        {
                            Id = _PhieuNhapThanhPham.Id,
                            MaNhap = _PhieuNhapThanhPham.MaNhap,
                            CreatedDate = _PhieuNhapThanhPham.CreatedDate,
                            NgayDuKienNhap = _PhieuNhapThanhPham.NgayDuKienNhap,
                            ModifiedDate = _PhieuNhapThanhPham.ModifiedDate,
                            CreatedBy = _PhieuNhapThanhPham.CreatedBy,
                            ModifiedBy = _PhieuNhapThanhPham.ModifiedBy,
                            DonViTienTe = _PhieuNhapThanhPham.DonViTienTe,

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
            PhieuNhapThanhPhamCRUDViewModel vm = await _context.PhieuNhapThanhPham.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            PhieuNhapThanhPhamCRUDViewModel vm = new PhieuNhapThanhPhamCRUDViewModel();

            if (id > 0)
            {
                vm = await _context.PhieuNhapThanhPham.Where(x => x.Id == id).SingleOrDefaultAsync();
                ViewBag.PhieuNhapThanhPham = JsonConvert.SerializeObject(GetThanhPhamNhapKhoByNhapThanhPhamId(id));
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "PhieuNhapThanhPham.PhieuNhapThanhPham.edit"))
                {
                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "PhieuNhapThanhPham.PhieuNhapThanhPham.edit_owner"))
                {
                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "PhieuNhapThanhPham.PhieuNhapThanhPham.create"))
                {
                    ViewBag.PhieuNhapThanhPham = JsonConvert.SerializeObject(Array.Empty<PhieuNhapThanhPhamCRUDViewModel>());
                    return PartialView("_AddEdit", vm);
                }
            }
            //[INSERT_OBJECTSUBVIEWFIELDLIST]
            return View("_AccessDeniedAddEdit");
        }
        private List<ListNhapThanhPham> GetThanhPhamNhapKhoByNhapThanhPhamId(int id)
        {
            var rs = from _tp in _context.ListNhapThanhPham
                     where _tp.IDPhieu == id && _tp.Cancelled == false
                     select new ListNhapThanhPham
                     {
                         ID = _tp.ID,
                         IDPhieu = _tp.IDPhieu,
                         IDNguyenLieu = _tp.IDNguyenLieu,
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

        //[INSERT_LOAD]

        [HttpPost]
        public async Task<JsonResult> AddEdit(PhieuNhapThanhPhamCRUDViewModel vm)
        {
            try
            {
                JsonResultViewModel _JsonResultViewModel = new();

                if (ModelState.IsValid)
                {
                    PhieuNhapThanhPham _PhieuNhapThanhPham = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _PhieuNhapThanhPham = await _context.PhieuNhapThanhPham.FindAsync(vm.Id);


                        vm.CreatedDate = _PhieuNhapThanhPham.CreatedDate;
                        vm.CreatedBy = _PhieuNhapThanhPham.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_PhieuNhapThanhPham).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        foreach (var item in vm.ThanhPhamNhapKhoList)
                        {
                            ListNhapThanhPham tp = new ListNhapThanhPham();
                            if (item.ID > 0)
                            {

                                tp = _context.ListNhapThanhPham.FirstOrDefault(x => x.ID == item.ID);
                                tp.Cancelled = item.Cancelled;
                                tp = item;

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
                                tp2 = _context.QuanLyThanhPham.FirstOrDefault(i => i.Id == tp.IDNguyenLieu);
                                tp2.SoLuong += tp.SoLuong;
                                _context.Update(tp2);
                                await _context.SaveChangesAsync();

                            }
                            else
                            {
                                tp = item;

                                tp.ID = item.ID;
                                tp.IDPhieu = _PhieuNhapThanhPham.Id;
                                tp.SoLuong = item.SoLuong;
                                tp.DonGia = item.DonGia;
                                tp.Cancelled = item.Cancelled;
                                tp.IDNguyenLieu = item.IDNguyenLieu;
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
                                tp2 = _context.QuanLyThanhPham.FirstOrDefault(i => i.Id == tp.IDNguyenLieu);
                                tp2.SoLuong += tp.SoLuong;
                                _context.Update(tp2);
                                await _context.SaveChangesAsync();
                            }

                        }
                        var _AlertMessage = "Đã thêm phiếu thành công";
                        return new JsonResult(_AlertMessage);
                       
                    }
                    else
                    {
                        _PhieuNhapThanhPham = vm;


                        _PhieuNhapThanhPham.CreatedDate = DateTime.Now;
                        _PhieuNhapThanhPham.ModifiedDate = DateTime.Now;
                        _PhieuNhapThanhPham.CreatedBy = _UserName;
                        _PhieuNhapThanhPham.ModifiedBy = _UserName;
                        _context.Add(_PhieuNhapThanhPham);
                        await _context.SaveChangesAsync();

                        foreach (var item in vm.ThanhPhamNhapKhoList)
                        {
                            ListNhapThanhPham tp = new ListNhapThanhPham();
                            tp = item;
                            tp.ID = item.ID;
                            tp.IDPhieu = _PhieuNhapThanhPham.Id;
                            tp.SoLuong = item.SoLuong;
                            tp.DonGia = item.DonGia;
                            tp.Cancelled = item.Cancelled;
                            tp.IDNguyenLieu = item.IDNguyenLieu;
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
                            tp2 = _context.QuanLyThanhPham.FirstOrDefault(i => i.Id == tp.IDNguyenLieu);
                            tp2.SoLuong += tp.SoLuong;
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
                var _PhieuNhapThanhPham = await _context.PhieuNhapThanhPham.FindAsync(id);
                _PhieuNhapThanhPham.ModifiedDate = DateTime.Now;
                _PhieuNhapThanhPham.ModifiedBy = HttpContext.User.Identity.Name;
                _PhieuNhapThanhPham.Cancelled = true;

                _context.Update(_PhieuNhapThanhPham);
                await _context.SaveChangesAsync();
                return new JsonResult(_PhieuNhapThanhPham);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsExists(long id)
        {
            return _context.PhieuNhapThanhPham.Any(e => e.Id == id);
        }

        [HttpGet]
        public IActionResult GetAllMaPhieuThanhPhamByMa(string ma)
        {
            var rs = (from _ma in _context.PhieuNhapThanhPham
                      where _ma.Cancelled == false && _ma.MaNhap.StartsWith(ma)
                      select new
                      {
                          MaNhap = _ma.MaNhap
                      }
                      ).ToList();
            return Ok(rs);
        }

        [HttpGet]
        public IActionResult GetListNguyenLieuNhapKhoLyThuyetKhiChuaNhapByID(int nhakholythuyet)
        {
            var a = (from _nl in _context.ListNhapThanhPham
                     join _ten in _context.PhieuNhapThanhPham on _nl.IDPhieu equals _ten.Id
                     where _nl.IDPhieu == nhakholythuyet
                     select new ListNhapThanhPhamCRUDViewModel
                     {
                         ID = _nl.ID,
                         IDPhieu = _nl.IDPhieu,
                         IDNguyenLieu = _nl.IDNguyenLieu,
                         PO = _nl.PO,
                         Mau = _nl.Mau,
                         Size = _nl.Size,
                         SoLuong = _nl.SoLuong,
                         DonGia = _nl.DonGia,
                         NgayNhap = _nl.NgayNhap,
                         KhachHang = _nl.KhachHang,
                         MaHang = _nl.MaHang,
                         Cancelled = _nl.Cancelled,
                         TenNguyenLieuByPhieuNhap = _ten.MaNhap,
                         TenDonViDisplay = _ten.DonViTienTe
                     }
                ).ToList();
            return Ok(a);
        }
        [HttpGet]
        public IActionResult GetListNhapKhoLyThuyetKhiChuaNhap()
        {
            var a = (from _nl in _context.PhieuNhapThanhPham
                     where _nl.Cancelled == false
                     select new PhieuNhapThanhPham
                     {
                         Id = _nl.Id,
                         MaNhap = _nl.MaNhap,
                         NgayDuKienNhap = _nl.NgayDuKienNhap,
                         DonViTienTe = _nl.DonViTienTe,
                     }
                ).ToList();
            return Ok(a);
        }
        [HttpPost]
        public async Task<JsonResult> AddThanhPhamWhenNhapKho([FromBody] List<QuanLyThanhPhamCRUDViewModel> ListThem)
        {
            try
            {
                JsonResultViewModel _JsonResultViewModel = new();
                string _UserName = user;
                foreach (var item in ListThem)
                {
                    QuanLyThanhPham _NguyenLieu = new();
                    _NguyenLieu = item;
                    _NguyenLieu.CreatedDate = DateTime.Now;
                    _NguyenLieu.ModifiedDate = DateTime.Now;
                    _NguyenLieu.CreatedBy = _UserName;
                    _NguyenLieu.ModifiedBy = _UserName;
                    _context.Add(_NguyenLieu);
                    await _context.SaveChangesAsync();
                }
                _JsonResultViewModel.ModelObject = ListThem;
                _JsonResultViewModel.AlertMessage = "Có lỗi khi thêm dữ liệu vào cơ sở dữ liệu";
                _JsonResultViewModel.IsSuccess = false;
                return new JsonResult(_JsonResultViewModel);
            }
            catch
            {
                JsonResultViewModel _JsonResultViewModel = new();
                _JsonResultViewModel.ModelObject = null;
                _JsonResultViewModel.AlertMessage = "Có lỗi khi thêm dữ liệu vào cơ sở dữ liệu";
                _JsonResultViewModel.IsSuccess = false;
                return new JsonResult(_JsonResultViewModel);
            }
        }

        public List<DtaPrint3> LoadPhieuNhapThanhPhamByNhapKhoId2(int IDPhieu)
        {
            return (from tblObj in _context.ListNhapThanhPham.Where(x => x.IDPhieu == IDPhieu && x.Cancelled == false).OrderBy(x => x.ID)
                    select new DtaPrint3
                    {
                        TenPhieu = _context.PhieuNhapThanhPham.FirstOrDefault(i => i.Id == tblObj.IDPhieu).MaNhap ?? "",
                        TenThanhPham = _context.QuanLyThanhPham.FirstOrDefault(t=>t.Id == tblObj.IDNguyenLieu).TenThanhPham ?? "",
                        PO = tblObj.PO,
                        Mau = tblObj.Mau,
                        Size = tblObj.Size,
                        DonGia = tblObj.DonGia,
                        SoLuong = tblObj.SoLuong
                    }
            ).ToList();
        }

        public IActionResult Print(int id)
        {
            ViewBag.Load = LoadPhieuNhapThanhPhamByNhapKhoId2(id);
            return PartialView("_Print");
        }
    }
    public class DtaPrint3
    {
        public string TenPhieu { get; set; }
        public string TenThanhPham { get; set; }
        public string PO { get; set; }
        public string Mau { get; set; }
        public string Size { get; set; }
        public int DonGia { get; set; }
        public int SoLuong { get; set; }

    }
}
