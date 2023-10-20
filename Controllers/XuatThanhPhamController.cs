using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.XuatThanhPhamViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
using AMS.Models.NhapThanhPhamViewModel;
using static System.Net.WebRequestMethods;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class XuatThanhPhamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;


        public XuatThanhPhamController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name ?? string.Empty;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "XuatThanhPham.XuatThanhPham.list"))
            {
                return View();
            }
            else if (_iCommon.CheckPermissionInProfile(user, "XuatThanhPham.XuatThanhPham.list_owner"))
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
                    || obj.NgayXuat.ToString().ToLower().Contains(searchValue)
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
        private IQueryable<XuatThanhPhamCRUDViewModel> GetGridItem()
        {
            if (_iCommon.CheckPermissionInProfile(user, "XuatThanhPham.XuatThanhPham.list_owner"))
            {
                try
                {
                    return (from _XuatThanhPham in _context.XuatThanhPham
                            where _XuatThanhPham.Cancelled == false && _XuatThanhPham.CreatedBy.Contains(user)

                            select new XuatThanhPhamCRUDViewModel
                            {
                                Id = _XuatThanhPham.Id,
                                NgayXuat = _XuatThanhPham.NgayXuat,
                                DonViTienTe = _XuatThanhPham.DonViTienTe,
                                CreatedDate = _XuatThanhPham.CreatedDate,
                                ModifiedDate = _XuatThanhPham.ModifiedDate,
                                CreatedBy = _XuatThanhPham.CreatedBy,
                                ModifiedBy = _XuatThanhPham.ModifiedBy,

                            }).OrderByDescending(x => x.Id);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            };


            try
            {
                return (from _XuatThanhPham in _context.XuatThanhPham
                        where _XuatThanhPham.Cancelled == false

                        select new XuatThanhPhamCRUDViewModel
                        {
                            Id = _XuatThanhPham.Id,
                            NgayXuat = _XuatThanhPham.NgayXuat,
                            DonViTienTe = _XuatThanhPham.DonViTienTe,
                            CreatedDate = _XuatThanhPham.CreatedDate,
                            ModifiedDate = _XuatThanhPham.ModifiedDate,
                            CreatedBy = _XuatThanhPham.CreatedBy,
                            ModifiedBy = _XuatThanhPham.ModifiedBy,

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
            XuatThanhPhamCRUDViewModel vm = await _context.XuatThanhPham.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            XuatThanhPhamCRUDViewModel vm = new XuatThanhPhamCRUDViewModel();

            if (id > 0)
            {
                vm = await _context.XuatThanhPham.Where(x => x.Id == id).SingleOrDefaultAsync();
                ViewBag.DanhSachThanhPham = JsonConvert.SerializeObject(GetThanhPhamXuatKhoByXuatThanhPhamId(id));
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "XuatThanhPham.XuatThanhPham.edit"))
                {
                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "XuatThanhPham.XuatThanhPham.edit_owner"))
                {
                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "XuatThanhPham.XuatThanhPham.create"))
                {
                    ViewBag.DanhSachThanhPham = JsonConvert.SerializeObject(Array.Empty<ThanhPhamXuatKhoCRUDViewModel>());
                    return PartialView("_AddEdit", vm);
                }
            }

            return View("_AccessDeniedAddEdit");

        }
        //[INSERT_LOAD]
        private List<ThanhPhamXuatKhoCRUDViewModel> GetThanhPhamXuatKhoByXuatThanhPhamId(int id)
        {
            var rs = from _tp in _context.ThanhPhamXuatKho
                     where _tp.XuatKhoThanhPhamId == id && _tp.Cancelled == false
                     select new ThanhPhamXuatKhoCRUDViewModel
                     {
                         Id = _tp.Id,
                         ThanhPhamId = _tp.ThanhPhamId,
                         SoLuong = _tp.SoLuong,
                         DonGia = _tp.DonGia,
                         PO = _tp.PO,
                         Size = _tp.Size,
                         Mau = _tp.Mau,
                         KhachHang = _tp.KhachHang,
                         MaHang = _tp.MaHang,
                         NgayXuat = _tp.NgayXuat,
                         Cancelled = _tp.Cancelled,
                         XuatKhoThanhPhamId = _tp.XuatKhoThanhPhamId
                     };
            return rs.ToList();
        }
        [HttpPost]
        public async Task<JsonResult> AddEdit(XuatThanhPhamCRUDViewModel vm)
        {
            try
            {
                JsonResultViewModel _JsonResultViewModel = new();

                if (ModelState.IsValid)
                {
                    XuatThanhPham _XuatThanhPham = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _XuatThanhPham = await _context.XuatThanhPham.FindAsync(vm.Id);


                        vm.CreatedDate = _XuatThanhPham.CreatedDate;
                        vm.CreatedBy = _XuatThanhPham.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_XuatThanhPham).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        foreach (var item in vm.ThanhPhamXuatKhoList)
                        {
                            ThanhPhamXuatKho tp = new ThanhPhamXuatKho();

                            if (item.Id > 0)
                            {
                                tp = _context.ThanhPhamXuatKho.FirstOrDefault(x => x.Id == item.Id);
                                tp.Cancelled = item.Cancelled;
                                tp.SoLuong = item.SoLuong;
                                tp.DonGia = item.DonGia;
                                tp.ThanhPhamId = item.ThanhPhamId;
                                tp.PO = item.PO;
                                tp.Mau = item.Mau;
                                tp.Size = item.Size;
                                tp.KhachHang = item.KhachHang;
                                tp.MaHang = item.MaHang;
                                tp.NgayXuat = item.NgayXuat;

                                tp.ModifiedDate = DateTime.Now;
                                tp.ModifiedBy = _UserName;
                                _context.Update(tp);
                                await _context.SaveChangesAsync();

                                QuanLyThanhPham tp2 = new QuanLyThanhPham();
                                tp2 = _context.QuanLyThanhPham.FirstOrDefault(i => i.Id == tp.ThanhPhamId);
                                tp2.SoLuong -= tp.SoLuong;
                                _context.Update(tp2);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                tp.Id = item.Id;
                                tp.XuatKhoThanhPhamId = _XuatThanhPham.Id;
                                tp.SoLuong = item.SoLuong;
                                tp.DonGia = item.DonGia;
                                tp.Cancelled = item.Cancelled;
                                tp.ThanhPhamId = item.ThanhPhamId;
                                tp.PO = item.PO;
                                tp.Mau = item.Mau;
                                tp.Size = item.Size;
                                tp.KhachHang = item.KhachHang;
                                tp.MaHang = item.MaHang;
                                tp.NgayXuat = item.NgayXuat;
                                tp.CreatedBy = _UserName;
                                tp.ModifiedBy = _UserName;
                                tp.CreatedDate = DateTime.Now;
                                tp.ModifiedDate = DateTime.Now;


                                await _context.AddAsync(tp);
                                await _context.SaveChangesAsync();

                                QuanLyThanhPham tp2 = new QuanLyThanhPham();
                                tp2 = _context.QuanLyThanhPham.FirstOrDefault(i => i.Id == tp.ThanhPhamId);
                                tp2.SoLuong -= tp.SoLuong;
                                _context.Update(tp2);
                                await _context.SaveChangesAsync();
                            }
                        }

                        var _AlertMessage = "Cập nhật thành công";
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _XuatThanhPham = vm;


                        _XuatThanhPham.CreatedDate = DateTime.Now;
                        _XuatThanhPham.ModifiedDate = DateTime.Now;
                        _XuatThanhPham.CreatedBy = _UserName;
                        _XuatThanhPham.ModifiedBy = _UserName;
                        _context.Add(_XuatThanhPham);
                        await _context.SaveChangesAsync();

                        foreach (var item in vm.ThanhPhamXuatKhoList)
                        {
                            ThanhPhamXuatKho tp = new ThanhPhamXuatKho();
                            tp = item;
                            tp.Id = item.Id;

                            tp.XuatKhoThanhPhamId = _XuatThanhPham.Id;
                            tp.CreatedBy = _UserName;
                            tp.ModifiedBy = _UserName;
                            tp.CreatedDate = DateTime.Now;
                            tp.ModifiedDate = DateTime.Now;
                            await _context.AddAsync(tp);
                            await _context.SaveChangesAsync();

                            QuanLyThanhPham tp2 = new QuanLyThanhPham();
                            tp2 = _context.QuanLyThanhPham.FirstOrDefault(i => i.Id == tp.ThanhPhamId);
                            tp2.SoLuong -= tp.SoLuong;
                            _context.Update(tp2);
                            await _context.SaveChangesAsync();
                        }
                        _JsonResultViewModel.ModelObject = vm;
                        _JsonResultViewModel.AlertMessage = "Đã tạo mới thành công";
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

            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "XuatThanhPham.XuatThanhPham.delete"))
            {
                try
                {
                    var _XuatThanhPham = await _context.XuatThanhPham.FindAsync(id);
                    _XuatThanhPham.ModifiedDate = DateTime.Now;
                    _XuatThanhPham.ModifiedBy = HttpContext.User.Identity.Name;
                    _XuatThanhPham.Cancelled = true;

                    _context.Update(_XuatThanhPham);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _XuatThanhPham;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Xóa thất bại";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = null;
                    return new JsonResult(_JsonResultViewModel);
                }
            }
            else if (_iCommon.CheckPermissionInProfile(user, "XuatThanhPham.XuatThanhPham.delete_owner"))
            {
                try
                {
                    var _XuatThanhPham = await _context.XuatThanhPham.FindAsync(id);
                    _XuatThanhPham.ModifiedDate = DateTime.Now;
                    _XuatThanhPham.ModifiedBy = HttpContext.User.Identity.Name;
                    _XuatThanhPham.Cancelled = true;

                    _context.Update(_XuatThanhPham);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _XuatThanhPham;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Xóa thất bại";
                    _JsonResultViewModel.IsSuccess = true;
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
            return _context.XuatThanhPham.Any(e => e.Id == id);
        }
        public List<DtaPrint4> LoadDanhNguyenLieuNhapKhoByNhapKhoId2(int NhapKhoLyThuyetId)
        {
            return (from tblObj in _context.ThanhPhamXuatKho.Where(x => x.XuatKhoThanhPhamId == NhapKhoLyThuyetId && x.Cancelled == false).OrderBy(x => x.Id)
                    join tbl in _context.XuatThanhPham on tblObj.XuatKhoThanhPhamId equals tbl.Id
                    select new DtaPrint4
                    {
                        NgayNhap = tblObj.NgayXuat,
                        KhachHang = tblObj.KhachHang,
                        MaHang = tblObj.MaHang,
                        TenThanhPham = _context.QuanLyThanhPham.FirstOrDefault(t => t.Id == tblObj.ThanhPhamId).TenThanhPham ?? "",
                        PO = tblObj.PO,
                        Mau = tblObj.Mau,
                        Size = tblObj.Size,
                        SoLuong = tblObj.SoLuong,
                        DonGia = tblObj.DonGia,
                        DonVi = tbl.DonViTienTe,
                        NgayNhapMain = tbl.NgayXuat,
                        ThanhTien = (tblObj.SoLuong * tblObj.DonGia) + tbl.DonViTienTe
                    }
            ).ToList();
        }

        public IActionResult Print(int id)
        {
            ViewBag.Load = LoadDanhNguyenLieuNhapKhoByNhapKhoId2(id);
            return PartialView("_Print");
        }
    }
}
