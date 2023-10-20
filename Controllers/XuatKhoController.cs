using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.XuatKhoViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
using AMS.Models.NguyenLieuXuatKhoCRUDViewModel;
using System.Globalization;
using AMS.Models.NhapKhoViewModel;
using Rotativa.AspNetCore;
using AMS.Models.KeHoachSanXuatViewModel;
using AMS.Models.NguyenLieuKeHoachCRUDViewModel;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class XuatKhoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;

        public XuatKhoController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name ?? string.Empty;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "XuatKho.XuatKho.list"))
            {
                return View();
            }
            else if (_iCommon.CheckPermissionInProfile(user, "XuatKho.XuatKho.list_owner"))
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
                    || obj.TenPhieuXuatKho.ToLower().Contains(searchValue)
                    || obj.DanhGia.ToLower().Contains(searchValue)
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

        private IQueryable<XuatKhoCRUDViewModel> GetGridItem()
        {

            if (_iCommon.CheckPermissionInProfile(user, "XuatKho.XuatKho.list_owner"))
            {
                try
                {
                    return (from _XuatKho in _context.XuatKho
                            where _XuatKho.Cancelled == false && _XuatKho.CreatedBy.Contains(user)

                            select new XuatKhoCRUDViewModel
                            {
                                Id = _XuatKho.Id,
                                TenPhieuXuatKho = _XuatKho.TenPhieuXuatKho,
                                DanhGia = _XuatKho.DanhGia,
                                CreatedDate = _XuatKho.CreatedDate,
                                ModifiedDate = _XuatKho.ModifiedDate,
                                CreatedBy = _XuatKho.CreatedBy,
                                ModifiedBy = _XuatKho.ModifiedBy,

                            }).OrderByDescending(x => x.Id);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            try
            {
                return (from _XuatKho in _context.XuatKho
                        where _XuatKho.Cancelled == false

                        select new XuatKhoCRUDViewModel
                        {
                            Id = _XuatKho.Id,
                            TenPhieuXuatKho = _XuatKho.TenPhieuXuatKho,
                            DanhGia = _XuatKho.DanhGia,
                            CreatedDate = _XuatKho.CreatedDate,
                            ModifiedDate = _XuatKho.ModifiedDate,
                            CreatedBy = _XuatKho.CreatedBy,
                            ModifiedBy = _XuatKho.ModifiedBy,

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
            XuatKhoCRUDViewModel vm = await _context.XuatKho.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            XuatKhoCRUDViewModel vm = new XuatKhoCRUDViewModel();
            if (id > 0)
            {
                vm = await _context.XuatKho.Where(x => x.Id == id).SingleOrDefaultAsync();
                ViewBag.LoadNguyenLieuXuatKho = JsonConvert.SerializeObject(LoadDanhNguyenLieuXuatKhoByXuatKhoId(id));

                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "XuatKho.XuatKho.edit"))
                {
                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "XuatKho.XuatKho.edit_owner"))
                {
                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "XuatKho.XuatKho.create"))
                {
                    ViewBag.LoadNguyenLieuXuatKho = JsonConvert.SerializeObject(Array.Empty<NguyenLieuXuatKhoCRUDViewModel>());
                    return PartialView("_AddEdit", vm);
                }
            }
            return View("_AccessDeniedAddEdit");
        }
        //[INSERT_LOAD]
        public IQueryable<NguyenLieuXuatKhoCRUDViewModel> LoadDanhNguyenLieuXuatKhoByXuatKhoId(int XuatKhoId)
        {
            return (from tblObj in _context.NguyenLieuXuatKho.Where(x => x.XuatKhoId == XuatKhoId && x.Cancelled == false).OrderBy(x => x.Id)
                    select new NguyenLieuXuatKhoCRUDViewModel
                    {
                        Id = tblObj.Id,
                        NguyenLieuId = tblObj.NguyenLieuId,
                        ChatLuong = tblObj.ChatLuong,
                        DonVi = tblObj.DonVi,
                        SoLuongXuat = tblObj.SoLuongXuat,
                        NgayXuat = tblObj.NgayXuat,
                        XuatKhoId = XuatKhoId,
                        ChiTietCusTom = tblObj.ChiTietCusTom

                    }
            );
        }
        [HttpPost]
        public async Task<JsonResult> AddEdit(XuatKhoCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    XuatKho _XuatKho = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _XuatKho = await _context.XuatKho.FindAsync(vm.Id);


                        vm.CreatedDate = _XuatKho.CreatedDate;
                        vm.CreatedBy = _XuatKho.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_XuatKho).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();
                        var _AlertMessage = "Chỉnh sửa thông tin thành công";

                        foreach (var item in vm.ListNguyenLieuXuatKho)
                        {
                            NguyenLieuXuatKho nl = new NguyenLieuXuatKho();
                            if (item.Id > 0)
                            {
                                nl = await _context.NguyenLieuXuatKho.FindAsync(item.Id);
                                item.ModifiedBy = _UserName;
                                item.ModifiedDate = DateTime.Now;
                                _context.Entry(nl).CurrentValues.SetValues(item);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                nl = item;
                                nl.NguyenLieuId = item.NguyenLieuId;
                                nl.XuatKhoId = _XuatKho.Id;
                                nl.NgayXuat = vm.NgayXuat;
                                nl.CreatedDate = DateTime.Now;
                                nl.ModifiedDate = DateTime.Now;
                                nl.CreatedBy = _UserName;
                                nl.ModifiedBy = _UserName;
                                _context.Add(nl);
                                await _context.SaveChangesAsync();
                            }
                        }


                        foreach (var item in vm.ListNguyenLieuXuatKho)
                        {
                            NguyenLieuXuatKho tp = item;

                            NguyenLieu nl = await _context.NguyenLieu.FindAsync(item.NguyenLieuId);
                            nl.SoLuong = nl.SoLuong - item.SoLuongXuat;
                            nl.SoLuongLyThuyet = nl.SoLuongLyThuyet - item.SoLuongXuat;

                            _context.Update(nl);
                            HistoryNhapXuatNguyenLieu hs = new HistoryNhapXuatNguyenLieu();

                            hs.NguyenLieuId = tp.NguyenLieuId;
                            hs.SoLuong = tp.SoLuongXuat;

                            hs.Status = false;
                            hs.Ngay = vm.NgayXuat;
                            _context.Add(hs);
                            await _context.SaveChangesAsync();
                        }
                        _AlertMessage = "Đã xuất nguyên liệu khỏi kho dữ liệu đã được cập nhật ";


                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _XuatKho = vm;
                        _XuatKho.CreatedDate = DateTime.Now;
                        _XuatKho.ModifiedDate = DateTime.Now;
                        _XuatKho.CreatedBy = _UserName;
                        _XuatKho.ModifiedBy = _UserName;
                        _context.Add(_XuatKho);
                        await _context.SaveChangesAsync();

                        foreach (var item in vm.ListNguyenLieuXuatKho)
                        {
                            NguyenLieuXuatKho nl = new NguyenLieuXuatKho();
                            nl = item;
                            nl.NguyenLieuId = item.NguyenLieuId;
                            nl.XuatKhoId = _XuatKho.Id;
                            nl.NgayXuat = vm.NgayXuat;
                            nl.CreatedDate = DateTime.Now;
                            nl.ModifiedDate = DateTime.Now;
                            nl.CreatedBy = _UserName;
                            nl.ModifiedBy = _UserName;
                            _context.Add(nl);
                            await _context.SaveChangesAsync();
                        }
                        var _AlertMessage = "Thêm mới phiếu xuất kho thành công";

                        foreach (var item in vm.ListNguyenLieuXuatKho)
                        {
                            NguyenLieuXuatKho tp = item;

                            NguyenLieu nl = await _context.NguyenLieu.FindAsync(item.NguyenLieuId);
                            nl.SoLuong = nl.SoLuong - item.SoLuongXuat;
                            nl.SoLuongLyThuyet = nl.SoLuongLyThuyet - item.SoLuongXuat;
                            _context.Update(nl);
                            HistoryNhapXuatNguyenLieu hs = new HistoryNhapXuatNguyenLieu();

                            hs.NguyenLieuId = tp.NguyenLieuId;
                            hs.SoLuong = tp.SoLuongXuat;
                            hs.Status = false;
                            hs.Ngay = vm.NgayXuat;
                            _context.Add(hs);
                            await _context.SaveChangesAsync();

                            if (tp.ChiTietCusTom != null && !(tp.ChiTietCusTom.Trim() == ""))
                            {
                                List<CustomFieldTotal> cus = new List<CustomFieldTotal>();
                                cus = JsonConvert.DeserializeObject<List<CustomFieldTotal>>(tp.ChiTietCusTom);
                                foreach (var itemCustom in cus)
                                {
                                    var list = _context.CustomFieldTotal;
                                    CustomFieldTotal value = new CustomFieldTotal();
                                    value = _context.CustomFieldTotal.FirstOrDefault(item => item.NguyenLieuID == nl.Id && item.ListCustom.Trim().Contains(itemCustom.ListCustom.Trim()));
                                    if (value != null)
                                    {
                                        value.QuantityProduct -= itemCustom.QuantityProduct;
                                        _context.Update(value);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }

                        }
                        _AlertMessage = "Nguyên liệu đã xuất khỏi kho dữ liệu đã được cập nhật lại";


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
        [HttpGet]
        public ActionResult<KeHoachSanXuatCRUDViewModel> GetApiCongThuc()
        {
            var x = from tp in _context.KeHoachSanXuat
                    join _listNguyenLieu in _context.NguyenLieuKeHoach on tp.Id equals _listNguyenLieu.KeHoachSanXuatId
                    join _nl in _context.NguyenLieu on _listNguyenLieu.NguyenLieuId equals _nl.Id

                    where tp.Cancelled == false
                    select new { tp, _listNguyenLieu, _nl };
            var result = new List<KeHoachSanXuatCRUDViewModel>();
            foreach (var item in x)
            {
                var keHoachSanXuatCRUDViewModel = new KeHoachSanXuatCRUDViewModel
                {
                    Id = item.tp.Id,
                    TenThanhPham = item.tp.TenThanhPham,
                    NgayDuKienHoan = item.tp.NgayDuKienHoan,
                    Cancelled = false,
                    ListNguyenLieuKeHoach = new List<NguyenLieuKeHoachCRUDViewModel>
                    {

                        new NguyenLieuKeHoachCRUDViewModel
                        {
                            Id = item._listNguyenLieu.Id,
                            NguyenLieuId = item._listNguyenLieu.NguyenLieuId,
                            //SoLuongCan = item._listNguyenLieu.SoLuongCan,
                            //ChatLuong = item._listNguyenLieu.ChatLuong,
                            //NgayDuKien = item._listNguyenLieu.NgayDuKien,
                            //KeHoachSanXuatId = item._listNguyenLieu.KeHoachSanXuatId,
                            //ThongSoKyThuat = item._listNguyenLieu.ThongSoKyThuat,
                            //ThanhPhan = item._listNguyenLieu.ThanhPhan,
                            //MaCode = item._listNguyenLieu.MaCode,
                            //Size = item._listNguyenLieu.Size,
                            //Unit = item._listNguyenLieu.Unit,
                            //TieuThu = item._listNguyenLieu.TieuThu,
                            //LoaiKeHoach = item._listNguyenLieu.LoaiKeHoach,

                        }
                    }

                };
                result.Add(keHoachSanXuatCRUDViewModel);
            }


            return Ok(result);
        }
        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {

            JsonResultViewModel _JsonResultViewModel = new();

            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "XuatKho.XuatKho.delete"))
            {
                try
                {
                    var _XuatKho = await _context.XuatKho.FindAsync(id);
                    _XuatKho.ModifiedDate = DateTime.Now;
                    _XuatKho.ModifiedBy = HttpContext.User.Identity.Name;
                    _XuatKho.Cancelled = true;

                    _context.Update(_XuatKho);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _XuatKho;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Xoá không thành công";
                    _JsonResultViewModel.IsSuccess = false;
                    _JsonResultViewModel.ModelObject = null;
                    return new JsonResult(_JsonResultViewModel);
                }
            }
            else if (_iCommon.CheckPermissionInProfile(user, "XuatKho.XuatKho.delete_owner"))
            {
                try
                {
                    var _XuatKho = await _context.XuatKho.FindAsync(id);
                    _XuatKho.ModifiedDate = DateTime.Now;
                    _XuatKho.ModifiedBy = HttpContext.User.Identity.Name;
                    _XuatKho.Cancelled = true;

                    _context.Update(_XuatKho);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _XuatKho;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Xoá không thành công";
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
            return _context.XuatKho.Any(e => e.Id == id);
        }

        public IActionResult Report(string startDateFormat, string endDateFormat)
        {
            DateTime startDate, endDate;

            if (string.IsNullOrEmpty(endDateFormat))
            {
                endDate = DateTime.Now;
            }
            else
            {
                endDate = DateTime.ParseExact(endDateFormat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            if (string.IsNullOrEmpty(startDateFormat))
            {
                startDate = DateTime.Now.AddDays(-30);
            }
            else
            {
                startDate = DateTime.ParseExact(startDateFormat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            List<ReportXuatKho> result = GetListReport(startDate, endDate);
            return View(result);
        }

        public List<ReportXuatKho> GetListReport(DateTime startDateFormat, DateTime endDateFormat)
        {
            var beforFiler = (from _nlXk in _context.NguyenLieuXuatKho
                              join _nl in _context.NguyenLieu
                              on _nlXk.NguyenLieuId equals _nl.Id
                              where _nlXk.Cancelled == false && (_nlXk.NgayXuat.Date >= startDateFormat.Date && _nlXk.NgayXuat.Date <= endDateFormat.Date)
                              select new ReportXuatKho
                              {
                                  TenNguyenLieu = _nl.TenNguyenLieu,
                                  MaNguyenLieu = _nl.MaNguyenLieu,
                                  ChatLuong = _nlXk.ChatLuong,
                                  SoLuongXuat = _nlXk.SoLuongXuat
                              }).ToList();

            var affterFilter = (beforFiler.GroupBy(x => x.TenNguyenLieu)
                                         .Select(g => new ReportXuatKho
                                         {
                                             TenNguyenLieu = g.Key,
                                             MaNguyenLieu = g.First().MaNguyenLieu,
                                             ChatLuong = g.First().ChatLuong,
                                             SoLuongXuat = g.Sum(x => x.SoLuongXuat)
                                         })).ToList();
            return affterFilter;
        }
        public IActionResult ReportXuatKho(string startDateFormat, string endDateFormat)
        {
            DateTime startDate = DateTime.ParseExact(startDateFormat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(endDateFormat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<ReportXuatKho> result = GetListReport(startDate, endDate);
            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;
            var pdf = new ViewAsPdf
            {
                ViewName = "_ReportXuatKho",
                Model = result,
                ViewData = ViewData
            };
            return pdf;
        }

        [HttpGet]
        public IActionResult Print(int id)
        {
            ViewBag.Load = LoadDanhNguyenLieuXuatKhoByXuatKhoId2(id);
            return PartialView("_Print");
        }

        public List<DtaPrint2> LoadDanhNguyenLieuXuatKhoByXuatKhoId2(int XuatKhoId)
        {
            return (from tblObj in _context.NguyenLieuXuatKho.Where(x => x.XuatKhoId == XuatKhoId && x.Cancelled == false).OrderBy(x => x.Id)
                    select new DtaPrint2
                    {
                        TenNl = _context.NguyenLieu.FirstOrDefault(i => i.Id == tblObj.NguyenLieuId)!.TenNguyenLieu,
                        SoLuong = tblObj.SoLuongXuat,
                        CL = tblObj.ChatLuong,
                        DV = tblObj.DonVi,

                    }
            ).ToList();
        }
        public class DtaPrint2
        {
            public string TenNl { get; set; }
            public int SoLuong { get; set; }
            public string CL { get; set; }
            public string DV { get; set; }
        }

        public IActionResult NguyenLieuXuatKhoAPI(string sart_date, string end_date)
        {
            bool check1 = sart_date == "0";
            bool check2 = end_date == "0";
            var rs = _context.NguyenLieuXuatKho.ToList();

            if (check1 || check2)
            {
                return Ok(rs);
            }

            DateTime startDate = DateTime.ParseExact(sart_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endtDate = DateTime.ParseExact(end_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var dsad = rs.Where(i => i.NgayXuat > startDate && i.NgayXuat < endtDate).ToList();

            return Ok(dsad);

        }

        [HttpGet]
        public IActionResult NguyenLieuXuatKhoAPI2(string end_date)
        {

            var rs = _context.NguyenLieuXuatKho.ToList();

            DateTime endtDate = DateTime.ParseExact(end_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var dsad = rs.Where(i => i.NgayXuat < endtDate).ToList();

            return Ok(dsad);

        }
    }

}
