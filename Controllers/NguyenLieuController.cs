using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.NguyenLieuViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ardalis.Extensions.StringManipulation;
using Nest;
using System.Linq;
using AMS.Models.CustomFieldTotalViewModel;
using AMS.Models.CustomFieldViewModel;
using AMS.Models.CustomFieldInputValueViewModel;
using AMS.Models.KhoXuongThucTeViewModel;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class NguyenLieuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;
        public readonly string user;

        public NguyenLieuController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
            user = accessor.HttpContext.User.Identity.Name ?? string.Empty;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NguyenLieu.NguyenLieu.list"))
            {
                return View();
            }
            else if (_iCommon.CheckPermissionInProfile(user, "NguyenLieu.NguyenLieu.list_owner"))
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
                    || obj.MaNguyenLieu.ToLower().Contains(searchValue)
                    || obj.TenNguyenLieu.ToLower().Contains(searchValue)
                    || obj.SoLuong.ToString().ToLower().Contains(searchValue)
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
        private IQueryable<NguyenLieuCRUDViewModel> GetGridItem()
        {
            if (_iCommon.CheckPermissionInProfile(user, "NguyenLieu.NguyenLieu.list_owner"))
            {
                try
                {
                    return (from _NguyenLieu in _context.NguyenLieu
                            where _NguyenLieu.Cancelled == false && _NguyenLieu.CreatedBy.Contains(user)

                            select new NguyenLieuCRUDViewModel
                            {
                                Id = _NguyenLieu.Id,
                                MaNguyenLieu = _NguyenLieu.MaNguyenLieu,
                                TenNguyenLieu = _NguyenLieu.TenNguyenLieu,
                                SoLuongLyThuyet = _NguyenLieu.SoLuongLyThuyet,
                                SoLuong = _NguyenLieu.SoLuong,
                                CreatedDate = _NguyenLieu.CreatedDate,
                                ModifiedDate = _NguyenLieu.ModifiedDate,
                                CreatedBy = _NguyenLieu.CreatedBy,
                                ModifiedBy = _NguyenLieu.ModifiedBy,

                            }).OrderByDescending(x => x.Id);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            };

            try
            {
                return (from _NguyenLieu in _context.NguyenLieu
                        where _NguyenLieu.Cancelled == false

                        select new NguyenLieuCRUDViewModel
                        {
                            Id = _NguyenLieu.Id,
                            MaNguyenLieu = _NguyenLieu.MaNguyenLieu,
                            TenNguyenLieu = _NguyenLieu.TenNguyenLieu,

                            SoLuong = _NguyenLieu.SoLuong,
                            CreatedDate = _NguyenLieu.CreatedDate,
                            ModifiedDate = _NguyenLieu.ModifiedDate,
                            CreatedBy = _NguyenLieu.CreatedBy,
                            ModifiedBy = _NguyenLieu.ModifiedBy,

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
            NguyenLieuCRUDViewModel vm = await _context.NguyenLieu.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }


        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            NguyenLieuCRUDViewModel vm = new NguyenLieuCRUDViewModel();
            List<KhoXuongThucTeCRUDViewModel> KhoXuongThucTeCRUDViewModelList = new List<KhoXuongThucTeCRUDViewModel>();
            KhoXuongThucTeCRUDViewModelList.Add(new KhoXuongThucTeCRUDViewModel
            {
                Id = 1,
                ParentId = 0,
                TenKho = "Root",
            });
            if (id > 0)
            {
                LoadKhoXuongThucTe(ref KhoXuongThucTeCRUDViewModelList, 1, 1, id);
            }
            else
            {
                LoadKhoXuongThucTe(ref KhoXuongThucTeCRUDViewModelList, 1, 1);
            }
            ViewBag._LoadKhoXuongThucTe = new SelectList(KhoXuongThucTeCRUDViewModelList, "Id", "TenKho");

            if (id > 0)
            {
                vm = await _context.NguyenLieu.Where(x => x.Id == id).SingleOrDefaultAsync();

                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NguyenLieu.NguyenLieu.edit"))
                {
                    return PartialView("_AddEdit", vm);
                }
                else if (_iCommon.CheckPermissionInProfile(user, "NguyenLieu.NguyenLieu.edit_owner"))
                {
                    return PartialView("_AddEdit", vm);
                }
            }
            else
            {
                if (user.Contains(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NguyenLieu.NguyenLieu.create"))
                {
                    return PartialView("_AddEdit", vm);
                }
            }

            return View("_AccessDeniedAddEdit");
        }

        // Thêm phần quản lý kho xưởng
        public IQueryable<KhoXuongThucTeCRUDViewModel> LoadKhoXuongThucTeByParentId(int ParentId, int ExcludeId = 0)
        {
            return (from _KhoXuongThucTe in _context.KhoXuongThucTe.Where(x => x.ParentId == ParentId && x.Id != ExcludeId && x.Cancelled == false)
                .OrderBy(x => x.Id)
                    select new KhoXuongThucTeCRUDViewModel
                    {
                        Id = _KhoXuongThucTe.Id,
                        ParentId = _KhoXuongThucTe.ParentId,
                        TenKho = _KhoXuongThucTe.TenKho
                    });
        }
        public void LoadKhoXuongThucTe(ref List<KhoXuongThucTeCRUDViewModel> KhoXuongThucTeCRUDViewModelList, int level = 0, int ParentId = 1, int ExcludeId = 0)
        {
            try
            {
                IQueryable<KhoXuongThucTeCRUDViewModel> _KhoXuongThucTeCRUDViewModelList = LoadKhoXuongThucTeByParentId(ParentId, ExcludeId);
                var level1 = level + 1;
                if (_KhoXuongThucTeCRUDViewModelList.Count() > 0)
                {
                    foreach (var item in _KhoXuongThucTeCRUDViewModelList)
                    {
                        item.TenKho = "---".Repeat((uint)level) + item.TenKho;
                        KhoXuongThucTeCRUDViewModelList.Add(item);
                        LoadKhoXuongThucTe(ref KhoXuongThucTeCRUDViewModelList, level1, item.Id, ExcludeId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public IActionResult GetApiCustomfield()
        {


            var a = from _nl in _context.CustomField
                    select new
                    {
                        ID = _nl.ID,
                        FieldName = _nl.FieldName,
                    };
            return Ok(a.ToList());

        }
        [AllowAnonymous]

        [HttpGet]
        public IActionResult GetApiCustomfieldValue()
        {


            var a = from _nl in _context.CustomFieldInputValue
                    select new
                    {
                        ID = _nl.ID,
                        CustomFieldKey = _nl.CustomFieldKey,
                        CustomFieldValue = _nl.CustomFieldValue,
                    };
            return Ok(a.ToList());

        }
        [AllowAnonymous]

        [HttpGet]
        public IActionResult GetApiCustomfieldValueTotal(int nguyenlieuId)
        {


            var a = from _nl in _context.CustomFieldTotal
                    where _nl.NguyenLieuID == nguyenlieuId && _nl.Cancel == false
                    select new
                    {
                        ID = _nl.ID,
                        NguyenLieuID = _nl.NguyenLieuID,
                        ListCustom = _nl.ListCustom,
                        QuantityProduct = _nl.QuantityProduct,
                        Cancel = _nl.Cancel,
                    };
            return Ok(a.ToList());

        }

        [HttpPost]
        public async Task<JsonResult> AddEdit(NguyenLieuCRUDViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    NguyenLieu _NguyenLieu = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _NguyenLieu = await _context.NguyenLieu.FindAsync(vm.Id);


                        vm.CreatedDate = _NguyenLieu.CreatedDate;
                        vm.CreatedBy = _NguyenLieu.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_NguyenLieu).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();

                        if (vm.ListCustom != null)
                        {
                            foreach (var custom in vm.ListCustom)
                            {
                                if (custom.ID == 0)
                                {
                                    CustomFieldTotal cus = new();
                                    cus = custom;
                                    cus.NguyenLieuID = _NguyenLieu.Id;
                                    cus.ListCustom = custom.ListCustom;
                                    cus.QuantityProduct = custom.QuantityProduct;
                                    _context.Add(cus);
                                    await _context.SaveChangesAsync();
                                }
                                else
                                {
                                    CustomFieldTotal cus = new();
                                    cus = _context.CustomFieldTotal.FirstOrDefault(it => it.ID == custom.ID);
                                    _context.Entry(cus).CurrentValues.SetValues(custom);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }

                        var _AlertMessage = "Nguyên Liệu: " + vm.TenNguyenLieu + " đã cập nhật thành công";
                        return new JsonResult(_AlertMessage);
                    }
                    else
                    {
                        _NguyenLieu = vm;
                        _NguyenLieu.CreatedDate = DateTime.Now;
                        _NguyenLieu.ModifiedDate = DateTime.Now;
                        _NguyenLieu.CreatedBy = _UserName;
                        _NguyenLieu.ModifiedBy = _UserName;
                        _context.Add(_NguyenLieu);
                        await _context.SaveChangesAsync();

                        if (vm.ListCustom != null)
                        {

                            foreach (var custom in vm.ListCustom)
                            {
                                CustomFieldTotal cus = new();
                                cus = custom;
                                cus.NguyenLieuID = _NguyenLieu.Id;
                                cus.ListCustom = custom.ListCustom;

                                cus.QuantityProduct = custom.QuantityProduct;
                                // khi có
                                _context.Add(cus);
                                await _context.SaveChangesAsync();
                            }
                        }

                        var _AlertMessage = "Nguyên liệu: " + vm.TenNguyenLieu + " đã thêm thành công";
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

            if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"], StringComparison.OrdinalIgnoreCase) || _iCommon.CheckPermissionInProfile(user, "NguyenLieu.NguyenLieu.delete"))
            {
                try
                {
                    var _NguyenLieu = await _context.NguyenLieu.FindAsync(id);
                    _NguyenLieu.ModifiedDate = DateTime.Now;
                    _NguyenLieu.ModifiedBy = HttpContext.User.Identity.Name;
                    _NguyenLieu.Cancelled = true;

                    _context.Update(_NguyenLieu);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _NguyenLieu;
                    return new JsonResult(_JsonResultViewModel);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else if (_iCommon.CheckPermissionInProfile(user, "NguyenLieu.NguyenLieu.delete_owner"))
            {
                try
                {
                    var _NguyenLieu = await _context.NguyenLieu.FindAsync(id);
                    _NguyenLieu.ModifiedDate = DateTime.Now;
                    _NguyenLieu.ModifiedBy = HttpContext.User.Identity.Name;
                    _NguyenLieu.Cancelled = true;

                    _context.Update(_NguyenLieu);
                    await _context.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Đã xóa thành công";
                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.ModelObject = _NguyenLieu;
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
            return _context.NguyenLieu.Any(e => e.Id == id);
        }

        [HttpGet]
        public IActionResult GetNguyenLieuAPI()
        {
            return Ok(GetGridItem());
        }
        [HttpGet]
        public IActionResult GetIdAndNameAndSoLuongDangCoNguyenLieuAPI()
        {
            var a = from _nl in _context.NguyenLieu
                    where _nl.Cancelled == false
                    select new
                    {
                        Id = _nl.Id,
                        Name = _nl.TenNguyenLieu,
                        SoLuongDangCo = _nl.SoLuong
                    };
            return Ok(a.ToList());

        }
        [HttpGet]
        public IActionResult GetIdAndNameNguyenLieuAPI()
        {
            var a = from _nl in _context.NguyenLieu
                    where _nl.Cancelled == false
                    select new
                    {
                        Id = _nl.Id,
                        Name = _nl.TenNguyenLieu,
                        SoLuongDangCo = _nl.SoLuong
                    };
            return Ok(a.ToList());

        }

        [HttpPost]
        public async Task<JsonResult> AddNguyenLieuWhenNhapKho([FromBody] List<NguyenLieuCRUDViewModel> ListThem)
        {
            try
            {
                JsonResultViewModel _JsonResultViewModel = new();
                string _UserName = user;
                foreach (var item in ListThem)
                {
                    NguyenLieu _NguyenLieu = new();
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

        [Authorize]
        public IActionResult ChiTiet(int id)
        {
            var nl = _context.NguyenLieu.FirstOrDefault(x => x.Id == id);

            return PartialView("_ChiTiet", nl);
        }

        //custom file
        [HttpPost]
        public IActionResult AddCustom(CustomFieldInputValueCRUDViewModel vm)
        {
            CustomFieldInputValue value = new();
            value = vm;
            _context.Add(value);
            _context.SaveChanges();
            return Ok(value);
        }



        //custom file
        [HttpPost]
        public IActionResult AddCustomTotal(CustomFieldTotalCRUDViewModel vm)
        {
            CustomFieldTotal value = new();
            value.ListCustom = vm.ListCustom;
            value.NguyenLieuID = vm.NguyenLieuID;
            value.QuantityProduct = vm.QuantityProduct;
            value.Cancel = false;
            _context.Add(value);
            _context.SaveChanges();
            return Ok(value);
        }
        [HttpPost]
        public IActionResult UpdateCustomTotal(CustomFieldTotalCRUDViewModel vm)
        {
            CustomFieldTotal value = new();
            value.QuantityProduct += vm.QuantityProduct;
            _context.Update(value);
            _context.SaveChanges();
            return Ok(value);
        }


        [HttpGet]
        public IActionResult CustomValue()
        {
            var ds = from _ct in _context.CustomFieldInputValue
                     select new
                     {
                         Id = _ct.ID,
                         CustomFildKey = _ct.CustomFieldKey,
                         CustomFieldValue = _ct.CustomFieldValue
                     };
            return Ok(ds.ToList());
        }

        [HttpGet]
        public IActionResult CustomValueTotalByNlId(int nlId)
        {
            var ds = from _ct in _context.CustomFieldTotal
                     where _ct.NguyenLieuID == nlId
                     select new
                     {
                         Id = _ct.ID,
                         NguyenLieuId = _ct.NguyenLieuID,
                         LisCustom = _ct.ListCustom,
                         Quantity = _ct.QuantityProduct
                     };
            return Ok(ds.ToList());
        }


        // report tồn kho
        public IActionResult Report()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DataReport(int khoId)
        {
            try
            {
                if (khoId != 0)
                {
                    var dsNl = from _nl in _context.NguyenLieu
                               where _nl.Cancelled == false && _nl.KhoId == khoId
                               select new
                               {
                                   id = _nl.Id,
                                   tenNl = _nl.TenNguyenLieu,
                                   maNl = _nl.MaNguyenLieu,
                                   soLuong = _nl.SoLuong,
                                   donViTinh = _nl.DonViTinh,
                                   khoId = _nl.KhoId,
                                   khoName = _context.KhoXuongThucTe.FirstOrDefault(i => i.Id == _nl.KhoId)!.TenKho,
                                   custom = _context.CustomFieldTotal.Where(i => i.NguyenLieuID == _nl.Id).ToList(),
                               };
                    return Ok(dsNl.ToList());
                }

                var ds = from _nl in _context.NguyenLieu
                         where _nl.Cancelled == false
                         select new
                         {
                             id = _nl.Id,
                             tenNl = _nl.TenNguyenLieu,
                             maNl = _nl.MaNguyenLieu,
                             soLuong = _nl.SoLuong,
                             donViTinh = _nl.DonViTinh,
                             khoId = _nl.KhoId,
                             khoName = _context.KhoXuongThucTe.FirstOrDefault(i => i.Id == _nl.KhoId)!.TenKho,
                             custom = _context.CustomFieldTotal.Where(i => i.NguyenLieuID == _nl.Id).ToList(),
                         };
                return Ok(ds.ToList());
            }
            catch
            {
                return Ok();
            }
        }

        [HttpGet]
        public IActionResult GetTotalKeyByNguyenLieuId(int nlid)
        {
            var result = from list in _context.CustomFieldTotal
                         where list.NguyenLieuID == nlid && list.Cancel == false
                         select new
                         {
                             ID = 0,
                             NguyenLieuID = nlid,
                             ListCustom = list.ListCustom,
                             QuantityProduct = 0,
                             Cancel = list.Cancel,
                         };

            return Json(result.ToList());
        }
    }

}
