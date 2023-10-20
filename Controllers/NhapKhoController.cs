using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.NhapKhoViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
using AMS.Models.NguyenLieuNhapKhoCRUDViewModel;
using Microsoft.Net.Http.Headers;
using System.Globalization;
using Aspose.Words.Lists;
using Rotativa.AspNetCore;
using AMS.Models.CustomFieldTotalViewModel;
using AMS.Models.TheoDoiChatLuongViewModel;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class NhapKhoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;

        public NhapKhoController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name ?? string.Empty;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapKho.NhapKho.list"))
            {
                return View();
            }
            else if (_iCommon.CheckPermissionInProfile(user, "NhapKho.NhapKho.list_owner"))
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
                    || obj.NgayNhap.ToString().ToLower().Contains(searchValue)
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
        private IQueryable<NhapKhoCRUDViewModel> GetGridItem()
        {
            if (_iCommon.CheckPermissionInProfile(user, "NhapKho.NhapKho.list_owner"))
            {
                try
                {
                    return (from _NhapKho in _context.NhapKho
                            where _NhapKho.Cancelled == false && _NhapKho.CreatedBy.Contains(user)

                            select new NhapKhoCRUDViewModel
                            {
                                Id = _NhapKho.Id,
                                NgayNhap = _NhapKho.NgayNhap,
                                MoTa = _NhapKho.MoTa,
                                DonViTienTe = _NhapKho.DonViTienTe,
                                CreatedDate = _NhapKho.CreatedDate,
                                ModifiedDate = _NhapKho.ModifiedDate,
                                CreatedBy = _NhapKho.CreatedBy,
                                ModifiedBy = _NhapKho.ModifiedBy,

                            }).OrderByDescending(x => x.Id);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            try
            {
                return (from _NhapKho in _context.NhapKho
                        where _NhapKho.Cancelled == false

                        select new NhapKhoCRUDViewModel
                        {
                            Id = _NhapKho.Id,
                            NgayNhap = _NhapKho.NgayNhap,
                            MoTa = _NhapKho.MoTa,
                            DonViTienTe = _NhapKho.DonViTienTe,
                            CreatedDate = _NhapKho.CreatedDate,
                            ModifiedDate = _NhapKho.ModifiedDate,
                            CreatedBy = _NhapKho.CreatedBy,
                            ModifiedBy = _NhapKho.ModifiedBy,

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
            NhapKhoCRUDViewModel vm = await _context.NhapKho.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            NhapKhoCRUDViewModel vm = new NhapKhoCRUDViewModel();
            if (id > 0)
            {
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapKho.NhapKho.edit"))
                {
                    vm = await _context.NhapKho.FirstOrDefaultAsync(m => m.Id == id);
                    ViewBag.LoadNguyenLieuNhapKho = JsonConvert.SerializeObject(LoadDanhNguyenLieuNhapKhoByNhapKhoId(id));
                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "NhapKho.NhapKho.edit_owner"))
                {
                    vm = await _context.NhapKho.FirstOrDefaultAsync(m => m.Id == id);
                    ViewBag.LoadNguyenLieuNhapKho = JsonConvert.SerializeObject(LoadDanhNguyenLieuNhapKhoByNhapKhoId(id));
                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                ViewBag.LoadNguyenLieuNhapKho = JsonConvert.SerializeObject(Array.Empty<NguyenLieuNhapKhoCRUDViewModel>());

                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapKho.NhapKho.create"))
                {
                    return PartialView("_AddEdit", vm);
                }
            }

            return View("_AccessDeniedAddEdit");
        }
        //[INSERT_LOAD]
        public IQueryable<NguyenLieuNhapKhoCRUDViewModel> LoadDanhNguyenLieuNhapKhoByNhapKhoId(int NhapKhoId)
        {
            return (from tblObj in _context.NguyenLieuNhapKho.Where(x => x.NhapKhoId == NhapKhoId && x.Cancelled == false).OrderBy(x => x.Id)
                    select new NguyenLieuNhapKhoCRUDViewModel
                    {
                        Id = tblObj.Id,
                        NguyenLieuId = tblObj.NguyenLieuId,
                        ChatLuong = tblObj.ChatLuong,
                        NhaCungCap = tblObj.NhaCungCap,
                        Solo = tblObj.Solo,
                        SoLuongNhap = tblObj.SoLuongNhap,
                        SoLuongNhapTrenChungTu = tblObj.SoLuongNhapTrenChungTu,
                        NgayNhap = tblObj.NgayNhap,
                        NhapKhoId = NhapKhoId,
                        DonGia = tblObj.DonGia,
                        ChiTietCusTom = tblObj.ChiTietCusTom,
                        MaKho = tblObj.MaKho,
                        MaHaiQuan = tblObj.MaHaiQuan,
                    }
            );
        }
        public List<DtaPrint> LoadDanhNguyenLieuNhapKhoByNhapKhoId2(int NhapKhoId)
        {
            return (from tblObj in _context.NguyenLieuNhapKho.Where(x => x.NhapKhoId == NhapKhoId && x.Cancelled == false).OrderBy(x => x.Id)
                    select new DtaPrint
                    {
                        MaHaiQuan = tblObj.MaHaiQuan,
                        MaKho = tblObj.MaKho,
                        TenNPL = _context.NguyenLieu.FirstOrDefault(i => i.Id == tblObj.NguyenLieuId).TenNguyenLieu ?? "",
                        SoLuongChungTu = tblObj.SoLuongNhapTrenChungTu,
                        SoLuongThucNhap = tblObj.SoLuongNhap,
                        DonGia = tblObj.DonGia,
                    }
            ).ToList();
        }

        [HttpPost]
        public async Task<JsonResult> AddEdit(NhapKhoCRUDViewModel vm)
        {
            try
            {
                JsonResultViewModel _JsonResultViewModel = new();
                if (ModelState.IsValid)
                {
                    NhapKho _NhapKho = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _NhapKho = await _context.NhapKho.FindAsync(vm.Id);
                        vm.CreatedDate = _NhapKho.CreatedDate;
                        vm.CreatedBy = _NhapKho.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_NhapKho).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        foreach (var item in vm.ListNguyenLieuNhapKho)
                        {
                            NguyenLieuNhapKho nl = new NguyenLieuNhapKho();
                            if (item.Id > 0)
                            {
                                nl = await _context.NguyenLieuNhapKho.FindAsync(item.Id);
                                item.ModifiedBy = _UserName;
                                item.ModifiedDate = DateTime.Now;
                                _context.Entry(nl).CurrentValues.SetValues(item);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                nl = item;
                                nl.NhapKhoId = _NhapKho.Id;
                                nl.NgayNhap = vm.NgayNhap;
                                nl.CreatedDate = DateTime.Now;
                                nl.ModifiedDate = DateTime.Now;
                                nl.CreatedBy = _UserName;
                                nl.ModifiedBy = _UserName;
                                _context.Add(nl);
                                await _context.SaveChangesAsync();
                            }
                        }


                        foreach (var item in vm.ListNguyenLieuNhapKho)
                        {
                            NguyenLieu nl2 = new NguyenLieu();
                            nl2 = _context.NguyenLieu.SingleOrDefault(x => x.Id == item.NguyenLieuId);
                            if (nl2 != null)
                            {
                                nl2.SoLuong += item.SoLuongNhap;
                                _context.NguyenLieu.Update(nl2);
                                await _context.SaveChangesAsync();
                            }
                        }

                        var _AlertMessage = "Đã nhập kho thành công";
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        string messageChenhLech = "";
                        _NhapKho = vm;
                        _NhapKho.CreatedDate = DateTime.Now;
                        _NhapKho.ModifiedDate = DateTime.Now;
                        _NhapKho.CreatedBy = _UserName;
                        _NhapKho.ModifiedBy = _UserName;
                        _context.Add(_NhapKho);
                        await _context.SaveChangesAsync();

                        foreach (var item in vm.ListNguyenLieuNhapKho)
                        {
                            NguyenLieuNhapKho nl = new NguyenLieuNhapKho();
                            nl = item;
                            nl.NhapKhoId = _NhapKho.Id;
                            nl.NgayNhap = vm.NgayNhap;
                            nl.CreatedDate = DateTime.Now;
                            nl.ModifiedDate = DateTime.Now;
                            nl.CreatedBy = _UserName;
                            nl.ModifiedBy = _UserName;
                            _context.Add(nl);
                            await _context.SaveChangesAsync();

                            string Nl = (await _context.NguyenLieu.FirstOrDefaultAsync(i => i.Id == nl.NguyenLieuId)).TenNguyenLieu;
                            if (nl.SoLuongNhap > nl.SoLuongNhapTrenChungTu)
                            {

                                messageChenhLech += "Số lượng thực nhập của " + Nl + " lớn hơn số lượng trên chứng từ/n";
                            }
                            else
                            {
                                messageChenhLech += "Số lượng thực nhập của " + Nl + " nhỏ hơn số lượng trên chứng từ/n";
                            }



                            NguyenLieu nl2 = new NguyenLieu();
                            nl2 = _context.NguyenLieu.SingleOrDefault(x => x.Id == item.NguyenLieuId);
                            if (nl2 != null)
                            {
                                nl2.SoLuong += nl.SoLuongNhap;
                                _context.NguyenLieu.Update(nl2);
                                await _context.SaveChangesAsync();

                                if (nl.ChiTietCusTom != null! && (nl.ChiTietCusTom.Trim() != ""))
                                {
                                    List<CustomFieldTotal> cus = new List<CustomFieldTotal>();
                                    cus = JsonConvert.DeserializeObject<List<CustomFieldTotal>>(nl.ChiTietCusTom);
                                    foreach (var itemCustom in cus)
                                    {
                                        var list = _context.CustomFieldTotal;
                                        CustomFieldTotal value = new CustomFieldTotal();
                                        value = _context.CustomFieldTotal.FirstOrDefault(item => item.NguyenLieuID == nl2.Id && item.ListCustom.Trim().Contains(itemCustom.ListCustom.Trim()));
                                        if (value != null)
                                        {
                                            value.QuantityProduct += itemCustom.QuantityProduct;
                                            _context.Update(value);
                                            await _context.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            _context.Add(itemCustom);
                                            await _context.SaveChangesAsync();
                                        }
                                    }
                                }

                            }
                            try
                            {
                                if (messageChenhLech != null && messageChenhLech != "")
                                {

                                    ThongBaoChenhLechNguyenLieu chenhLechNguyenLieu = new ThongBaoChenhLechNguyenLieu();
                                    chenhLechNguyenLieu.CreatedBy = user ?? "";
                                    chenhLechNguyenLieu.ModifiedBy = user ?? "";
                                    chenhLechNguyenLieu.ModifiedDate = DateTime.Now;
                                    chenhLechNguyenLieu.CreatedDate = _NhapKho.NgayNhap;
                                    chenhLechNguyenLieu.NoiDung = messageChenhLech.ToString();
                                    await _context.AddAsync(chenhLechNguyenLieu);
                                    await _context.SaveChangesAsync();
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {

                            }
                        }

                        _JsonResultViewModel.ModelObject = vm;
                        _JsonResultViewModel.AlertMessage = "Đã thêm vào kho thành công, dữ liệu nguyên liệu đã được cập nhập";
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

            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NhapKho.NhapKho.delete"))
            {
                try
                {
                    var _NhapKho = await _context.NhapKho.FindAsync(id);
                    _NhapKho.ModifiedDate = DateTime.Now;
                    _NhapKho.ModifiedBy = HttpContext.User.Identity.Name;
                    _NhapKho.Cancelled = true;

                    _context.Update(_NhapKho);
                    await _context.SaveChangesAsync();

                    List<NguyenLieuNhapKho> listNl = await _context.NguyenLieuNhapKho.Where(l => l.NhapKhoId == id).ToListAsync();
                    if (listNl != null)
                    {
                        foreach (var item in listNl)
                        {
                            NguyenLieu nl2 = new NguyenLieu();
                            nl2 = _context.NguyenLieu.SingleOrDefault(x => x.Id == item.NguyenLieuId);
                            item.Cancelled = false;
                            nl2.SoLuong = nl2.SoLuong - item.SoLuongNhap;
                            _context.Update(item);
                            await _context.SaveChangesAsync();
                        }
                    }
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _NhapKho;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = null;
                    return new JsonResult(_JsonResultViewModel);
                }
            }
            else if (_iCommon.CheckPermissionInProfile(user, "NhapKho.NhapKho.delete_owner"))
            {
                try
                {
                    var _NhapKho = await _context.NhapKho.FindAsync(id);
                    _NhapKho.ModifiedDate = DateTime.Now;
                    _NhapKho.ModifiedBy = HttpContext.User.Identity.Name;
                    _NhapKho.Cancelled = true;

                    _context.Update(_NhapKho);
                    await _context.SaveChangesAsync();
                    List<NguyenLieuNhapKho> listNl = await _context.NguyenLieuNhapKho.Where(l => l.NhapKhoId == id).ToListAsync();
                    foreach (var item in listNl)
                    {
                        NguyenLieu nl2 = new NguyenLieu();
                        nl2 = _context.NguyenLieu.SingleOrDefault(x => x.Id == item.NguyenLieuId);
                        item.Cancelled = false;
                        nl2.SoLuong = nl2.SoLuong - item.SoLuongNhap;
                        _context.Update(item);
                        await _context.SaveChangesAsync();
                    }
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _NhapKho;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            _JsonResultViewModel.AlertMessage = "Bạn không có quyền xóa";
            _JsonResultViewModel.IsSuccess = true;
            _JsonResultViewModel.ModelObject = null;

            return new JsonResult(_JsonResultViewModel);
        }

        private bool IsExists(long id)
        {
            return _context.NhapKho.Any(e => e.Id == id);
        }

        [HttpGet]
        public IActionResult DownLoadFileExcel()
        {
            string filePath = _iCommon.GetContentRootPath() + $"wwwroot\\upload\\excel\\Example.xlsx";
            string fileName = "Example.xlsx";

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fileStream, new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
            {
                FileDownloadName = fileName
            };
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
            List<ReportNhapKho> result = GetListReport(startDate, endDate);
            return View(result);
        }

        public List<ReportNhapKho> GetListReport(DateTime startDateFormat, DateTime endDateFormat)
        {
            var beforFiler = (from _nlNk in _context.NguyenLieuNhapKho
                              join _nl in _context.NguyenLieu
                              on _nlNk.NguyenLieuId equals _nl.Id
                              where _nlNk.Cancelled == false && (_nlNk.NgayNhap >= startDateFormat && _nlNk.NgayNhap <= endDateFormat)
                              select new ReportNhapKho
                              {
                                  MaNguyenLieu = _nl.MaNguyenLieu,
                                  TenNguyenLieu = _nl.TenNguyenLieu,
                                  DonGia = _nlNk.DonGia,
                                  SoLuongNhap = _nlNk.SoLuongNhap
                              }).ToList();

            var affterFilter = (beforFiler.GroupBy(x => x.TenNguyenLieu)
                                         .Select(g => new ReportNhapKho
                                         {
                                             TenNguyenLieu = g.Key,
                                             MaNguyenLieu = g.First().MaNguyenLieu,
                                             DonGia = g.First().DonGia,
                                             SoLuongNhap = g.Sum(x => x.SoLuongNhap)
                                         })).ToList();
            return affterFilter;
        }

        public IActionResult ReportNhapKho(string startDateFormat, string endDateFormat)
        {
            DateTime startDate = DateTime.ParseExact(startDateFormat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(endDateFormat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<ReportNhapKho> result = GetListReport(startDate, endDate);
            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;
            var pdf = new ViewAsPdf
            {
                ViewName = "_ReportNhapKho",
                Model = result,
                ViewData = ViewData
            };
            return pdf;
        }

        [HttpPost]
        public async Task<IActionResult> InsertCustom([FromForm] List<CustomFieldTotalCRUDViewModel> vm)
        {
            foreach (var item in vm)
            {
                CustomFieldTotal ct = new();
                ct = item;
                await _context.AddAsync(ct);
                await _context.SaveChangesAsync();
            }
            return Ok(vm);
        }
        [HttpPost]
        public async Task<IActionResult> InsertCustomReportProduct([FromForm] List<TheoDoiChatLuongCRUDViewModel> vm)
        {

            foreach (var item in vm)
            {
                if (item.ChatLuongDrop != 0)
                {
                    TheoDoiChatLuong ct = new();
                    ct.GhiChuVeChatLuong = "Không có ghi chú gì";
                    ct.ChatLuongDrop = item.ChatLuongDrop;
                    ct.TenNguyenLieuId = item.TenNguyenLieuId;
                    ct.CreatedBy = user ?? "";
                    ct.ModifiedBy = user ?? "";
                    ct.ModifiedDate = DateTime.Now;
                    ct.CreatedDate = DateTime.Now;
                    await _context.AddAsync(ct);
                    await _context.SaveChangesAsync();
                }
            }
            return Ok(vm);
        }
        [HttpGet]
        public IActionResult Print(int id)
        {
            ViewBag.Load = LoadDanhNguyenLieuNhapKhoByNhapKhoId2(id);
            return PartialView("_Print");
        }

        [HttpGet]
        public IActionResult NguyenLieuNhapKhoAPI(string sart_date, string end_date)
        {
            bool check1 = sart_date == "0";
            bool check2 = end_date == "0";
            var rs = _context.NguyenLieuNhapKho.ToList();

            if (check1 || check2)
            {
                return Ok(rs);
            }

            DateTime startDate = DateTime.ParseExact(sart_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endtDate = DateTime.ParseExact(end_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var dsad = rs.Where(i => i.NgayNhap > startDate && i.NgayNhap < endtDate).ToList();

            return Ok(dsad);

        }

        [HttpGet]
        public IActionResult NguyenLieuNhapKhoAPI2(string end_date)
        {

            var rs = _context.NguyenLieuNhapKho.ToList();

            DateTime endtDate = DateTime.ParseExact(end_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var dsad = rs.Where(i => i.NgayNhap < endtDate).ToList();

            return Ok(dsad);

        }

    }
    public class CustomFieldTotal2
    {
        public int ID { get; set; }
        public int NguyenLieuID { get; set; }
        public List<int> ListCustom { get; set; }
        public int QuantityProduct { get; set; }
        public bool Cancel { get; set; }
    }
    public class DtaPrint
    {
        public int DonGia { get; set; }
        public int SoLuongThucNhap { get; set; }
        public int SoLuongChungTu { get; set; }
        public int QuantitTenNPLyProduct { get; set; }
        public string MaKho { get; set; }
        public string MaHaiQuan { get; set; }
        public string TenNPL { get; set; }
    }

}
