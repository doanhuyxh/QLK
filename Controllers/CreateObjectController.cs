using AMS.Data;
using AMS.Models;
using AMS.Services;
using AMS.Models.CreateObjectViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using AMS.Models.CommonViewModel;
using System.Globalization;

using Microsoft.AspNetCore.Mvc.Rendering;
using AMS.Helpers;
using AMS.Models.ObjectFieldViewModel;
using Newtonsoft.Json;

using Microsoft.EntityFrameworkCore.Metadata;

using System.Data;
using AMS.Models.DisplayFieldOfTableViewModel;
using AMS.Models.DisplaySubViewFieldOfTableViewModel;
using Nest;
using AMS.Models.SubViewViewModel;
//using static AMS.Pages.MainMenu;
using NuGet.Configuration;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.RegularExpressions;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class CreateObjectController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;
        private readonly IConfiguration _configuration;

        public CreateObjectController(ApplicationDbContext context, ICommon iCommon, IConfiguration configuration)
        {
            _context = context;
            _iCommon = iCommon;
            _configuration = configuration;
        }

        //[Authorize(Roles = AMS.Pages.MainMenu.CreateObject.RoleName)]
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

        private IQueryable<CreateObjectCRUDViewModel> GetGridItem()
        {
            try
            {
                return (from _CreateObject in _context.CreateObject
                        where _CreateObject.Cancelled == false && _CreateObject.Id != 102
                        select new CreateObjectCRUDViewModel
                        {
                            Id = _CreateObject.Id,
                            Name = _CreateObject.Name,
                            Label = _CreateObject.Label,
                            Description = _CreateObject.Description,
                            CreatedDate = _CreateObject.CreatedDate,
                            ModifiedDate = _CreateObject.ModifiedDate,
                            CreatedBy = _CreateObject.CreatedBy,
                            ModifiedBy = _CreateObject.ModifiedBy,

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
            CreateObjectCRUDViewModel vm = await _context.CreateObject.FirstOrDefaultAsync(m => m.Id == id);
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            //get a string value

            var ListTable = _iCommon.LoadTablesName();
            CreateObjectCRUDViewModel vm = new CreateObjectCRUDViewModel();
            if (id > 0)
            {
                vm = await _context.CreateObject.Where(x => x.Id == id).SingleOrDefaultAsync();
                vm.ObjectFields = GetListObjectFieldsCreateObjectId(vm.Id).ToList();
                if (vm.ObjectFields.Count() == 0)
                {
                    string TenBang = vm.Name;
                    if (TenBang.Any(Char.IsWhiteSpace))
                    {
                        TenBang = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(vm.Name.ToLower());
                        TenBang = Utility.RemoveUnicode(TenBang);
                        TenBang = String.Concat(TenBang.Where(c => !Char.IsWhiteSpace(c)));
                    }
                    List<string> ListFielBase = new List<string>();
                    ListFielBase.Add("CreatedDate");
                    ListFielBase.Add("ModifiedDate");
                    ListFielBase.Add("CreatedBy");
                    ListFielBase.Add("ModifiedBy");
                    ListFielBase.Add("Cancelled");
                    ListFielBase.Add("Id");
                    List<string> listField = _iCommon.LoadFieldsByTableName(TenBang);
                    foreach (var field in listField)
                    {
                        if (!ListFielBase.Contains(field))
                        {
                            vm.ObjectFields.Add(new ObjectFieldCRUDViewModel
                            {
                                CreateObjectId = vm.Id,
                                RowIndex = -1,
                                ColumnIndex = -1,
                                TenTruong = field,
                                HienThiTrongBang = true,
                                BatBuocNhap = true,
                                Label = "",
                                DoLon = 20,
                                KieuDuLieu = "string",
                                TreeView = false,
                            });
                        }

                    }


                }
                else
                {
                    for (var i = 0; i < vm.ObjectFields.Count; i++)
                    {
                        var ObjectField = vm.ObjectFields[i];
                        ObjectField.ShowSubItem = true;
                        var sub_items = LoadSubViewCRUDViewModelByOjbectFieldId(ObjectField.Id).ToList();

                        for (var i_sub_item = 0; i_sub_item < sub_items.Count; i_sub_item++)
                        {
                            var TenTruong = sub_items[i_sub_item].TenTruong;
                            //sub_items[i_sub_item].ListFieds = ListTable.Values;

                        }
                        vm.ObjectFields[i].subItemsNeedDelete = new List<int>();
                        vm.ObjectFields[i].sub_items = sub_items;

                    }
                }


                ViewBag.JsonObjectFields = JsonConvert.SerializeObject(vm.ObjectFields);
            }
            else
            {
                ViewBag.JsonObjectFields = JsonConvert.SerializeObject(Array.Empty<ObjectField>());
            }
            ViewBag._LoadObjectView = new SelectList(_iCommon.LoadObjectView(), "Id", "Name");
            ViewBag.JsonTablesName = JsonConvert.SerializeObject(ListTable);
            ViewBag.ListViewTemplate = JsonConvert.SerializeObject(_iCommon.LoadObjectView());
            return PartialView("_AddEdit", vm);
        }
        private List<ObjectFieldCRUDViewModel> GetListObjectFieldsCreateObjectId(int CreateObjectId)
        {
            List<ObjectFieldCRUDViewModel> ObjectFieldCRUDViewModelList = new List<ObjectFieldCRUDViewModel>();
            try
            {
                return _context.ObjectField.Where(x => x.Cancelled == false && x.CreateObjectId == CreateObjectId).AsEnumerable<ObjectField>().GroupJoin(_context.DisplayFieldOfTable, ObjectField => ObjectField.Id, DisplayFieldOfTable => DisplayFieldOfTable.ObjectFiledId,
                (ObjectField, DisplayFieldOfTable) => new ObjectFieldCRUDViewModel
                {
                    Id = ObjectField.Id,
                    CreateObjectId = ObjectField.CreateObjectId,
                    Label = ObjectField.Label,
                    TenBang = ObjectField.TenBang != null ? ObjectField.TenBang : "",
                    LabelTenBang = ObjectField.LabelTenBang != null ? ObjectField.LabelTenBang : "",
                    subItemsNeedDelete = new List<int>(),
                    json_sub_items = "",
                    RowIndex = ObjectField.RowIndex,
                    ColumnIndex = ObjectField.ColumnIndex,
                    TenTruong = ObjectField.TenTruong,
                    HienThiTrongBang = ObjectField.HienThiTrongBang,
                    BatBuocNhap = ObjectField.BatBuocNhap,
                    DoLon = ObjectField.DoLon,
                    KieuDuLieu = ObjectField.KieuDuLieu,
                    CreatedDate = ObjectField.CreatedDate,
                    ModifiedDate = ObjectField.ModifiedDate,
                    CreatedBy = ObjectField.CreatedBy,
                    ModifiedBy = ObjectField.ModifiedBy,
                    Cancelled = ObjectField.Cancelled,
                    TreeView = ObjectField.TreeView,
                    ListDisplayFieldOfTable = DisplayFieldOfTable.ToList<DisplayFieldOfTable>()
                }).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<SubViewCRUDViewModel> LoadSubViewCRUDViewModelByOjbectFieldId(int ObjectFieldId)
        {
            List<SubViewCRUDViewModel> ObjectFieldCRUDViewModelList = new List<SubViewCRUDViewModel>();
            try
            {
                return _context.SubViewField.Where(x => x.Cancelled == false && x.OjbectFieldId == ObjectFieldId).AsEnumerable<SubViewField>().GroupJoin(
                    _context.DisplaySubViewFieldOfTable,
                    _SubViewField => _SubViewField.Id,
                    DisplaySubViewFieldOfTable => DisplaySubViewFieldOfTable.ObjectSubViewFiledId,
                (_SubViewField, DisplaySubViewFieldOfTable) => new SubViewCRUDViewModel
                {
                    Id = _SubViewField.Id,
                    OjbectFieldId = _SubViewField.OjbectFieldId,
                    Label = _SubViewField.Label,
                    TenTruong = _SubViewField.TenTruong,
                    HienThiTrongBang = _SubViewField.HienThiTrongBang,
                    BatBuocNhap = _SubViewField.BatBuocNhap,
                    DoLon = _SubViewField.DoLon,
                    KieuDuLieu = _SubViewField.KieuDuLieu,
                    CreatedDate = _SubViewField.CreatedDate,
                    ModifiedDate = _SubViewField.ModifiedDate,
                    CreatedBy = _SubViewField.CreatedBy,
                    ModifiedBy = _SubViewField.ModifiedBy,
                    Cancelled = _SubViewField.Cancelled,
                    TreeView = _SubViewField.TreeView,
                    ListFieds = new List<DisplaySubViewFieldOfTableCRUDViewModel>(),
                    ListDisplaySubViewFieldOfTable = DisplaySubViewFieldOfTable.ToList<DisplaySubViewFieldOfTable>()
                }).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> AddEdit(CreateObjectCRUDViewModel vm)
        {



            var DatabaseName = _iCommon.GetDatabaseName();

            JsonResultViewModel _JsonResultViewModel = new();

            try
            {
                var ProjectPath = _iCommon.GetAppDevPath();

                int CreateObjectId = vm.CreateObjectId;
                CreateObjectCRUDViewModel CopyCreateObjectCRUDViewModel = await _context.CreateObject.Where(x => x.Id == CreateObjectId).SingleOrDefaultAsync();
                var ViewSource = CopyCreateObjectCRUDViewModel.Name;
                if (ModelState.IsValid)
                {
                    CreateObject _CreateObject = new();
                    string _UserName = HttpContext.User.Identity.Name;
                    if (vm.Id > 0)
                    {
                        _CreateObject = await _context.CreateObject.FindAsync(vm.Id);

                        vm.CreatedDate = _CreateObject.CreatedDate;
                        vm.CreatedBy = _CreateObject.CreatedBy;
                        vm.ModifiedDate = DateTime.Now;
                        vm.ModifiedBy = _UserName;
                        _context.Entry(_CreateObject).CurrentValues.SetValues(vm);
                        await _context.SaveChangesAsync();


                        List<ObjectFieldCRUDViewModel> ObjectFields = vm.ObjectFields;
                        List<string> ListUpdateField = new List<string>();
                        List<string> ListInsertField = new List<string>();
                        foreach (var item in ObjectFields)
                        {
                            ObjectField _ObjectField = new();
                            item.CreateObjectId = vm.Id;
                            if (item.Id > 0)
                            {
                                _ObjectField = await _context.ObjectField.FindAsync(item.Id);
                                ObjectField _OldObjectField = _ObjectField;
                                item.ModifiedBy = _UserName;
                                if (item.KieuDuLieu == "dropdown")
                                {
                                    item.TreeView = item.ListFieds.Where(field => field.TenTruong == "ParentId").Count() >= 1;
                                }
                                else
                                {
                                    item.TreeView = false;
                                }
                                _context.Entry(_ObjectField).CurrentValues.SetValues(item);
                                await _context.SaveChangesAsync();
                                if (_ObjectField.TenTruong != _OldObjectField.TenTruong)
                                {
                                    ListUpdateField.Add($"EXEC {DatabaseName}.sys.sp_rename N'{DatabaseName}.dbo.{_CreateObject.Name}.{_OldObjectField.TenTruong}' , N'{item.TenTruong}', 'COLUMN'");
                                }
                                string BatBuocNhap = _ObjectField.BatBuocNhap ? "NOT" : "";
                                switch (_ObjectField.KieuDuLieu)
                                {
                                    case "dropdown":
                                        ListUpdateField.Add(@$"
                                            IF COL_LENGTH('{DatabaseName}.dbo.{_CreateObject.Name}','{_OldObjectField.TenTruong}') IS NOT NULL
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ALTER COLUMN {_OldObjectField.TenTruong} int {BatBuocNhap} NULL
                                            ELSE 
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ADD {_OldObjectField.TenTruong} int {BatBuocNhap} NULL
                                            ");
                                        break;
                                    case "date":
                                    case "datetime":
                                        ListUpdateField.Add(@$"
                                            IF COL_LENGTH('{DatabaseName}.dbo.{_CreateObject.Name}','{_OldObjectField.TenTruong}') IS NOT NULL
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ALTER COLUMN {_OldObjectField.TenTruong} datetime2(0) {BatBuocNhap} NULL
                                            ELSE 
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ADD {_OldObjectField.TenTruong} datetime2(0) {BatBuocNhap} NULL
                                            ");

                                        break;
                                    case "bolean":
                                        ListUpdateField.Add(@$"
                                            IF COL_LENGTH('{DatabaseName}.dbo.{_CreateObject.Name}','{_OldObjectField.TenTruong}') IS NOT NULL
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ALTER COLUMN {_OldObjectField.TenTruong} bit {BatBuocNhap} NULL
                                            ELSE 
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ADD {_OldObjectField.TenTruong} bit {BatBuocNhap} NULL
                                            ");



                                        break;
                                    case "ParentId":
                                        ListUpdateField.Add(@$"
                                            IF COL_LENGTH('{DatabaseName}.dbo.{_CreateObject.Name}','ParentId') IS NULL
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ADD ParentId int NOT NULL
                                            ");

                                        break;
                                    case "image":
                                        ListUpdateField.Add(@$"
                                            IF COL_LENGTH('{DatabaseName}.dbo.{_CreateObject.Name}','{_OldObjectField.TenTruong}Path') IS NOT NULL
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ALTER COLUMN ParentId nvarchar(200) {BatBuocNhap} NULL
                                            ELSE 
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ADD {_OldObjectField.TenTruong}Path nvarchar(200) {BatBuocNhap} NULL
                                            ");
                                        break;
                                    case "table":
                                        //do not something

                                        break;
                                    default:
                                        ListUpdateField.Add(@$"
                                            IF COL_LENGTH('{DatabaseName}.dbo.{_CreateObject.Name}','{_OldObjectField.TenTruong}') IS NOT NULL
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ALTER COLUMN {_OldObjectField.TenTruong} nvarchar({item.DoLon}) {BatBuocNhap} NULL
                                            ELSE 
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ADD {_OldObjectField.TenTruong} nvarchar(200) {BatBuocNhap} NULL
                                            ");

                                        break;
                                }
                                switch (_ObjectField.KieuDuLieu)
                                {
                                    case "table":
                                        await DeleteSubViewFieldNeedDelete(_ObjectField.Id, item);
                                        await UpdateSubView(_ObjectField.Id, item);
                                        await InsertSubView(_CreateObject, _ObjectField.Id, item);
                                        break;
                                }
                            }
                            else
                            {
                                item.CreatedDate = _CreateObject.CreatedDate;
                                item.CreatedBy = _CreateObject.CreatedBy;
                                item.ModifiedDate = DateTime.Now;
                                item.ModifiedBy = _UserName;
                                if (item.KieuDuLieu == "dropdown")
                                {
                                    item.TreeView = item.ListFieds.Where(field => field.TenTruong == "ParentId").Count() >= 1;
                                }
                                _ObjectField = item;
                                _context.Add(_ObjectField);
                                await _context.SaveChangesAsync();
                                string BatBuocNhap = item.BatBuocNhap ? "NOT" : "";
                                switch (item.KieuDuLieu)
                                {
                                    case "dropdown":
                                        ListInsertField.Add(@$"
                                            IF COL_LENGTH('{DatabaseName}.dbo.{_CreateObject.Name}','{item.TenTruong}Id') IS NOT NULL
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ALTER COLUMN {item.TenTruong}Id int {BatBuocNhap} NULL
                                            ELSE 
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ADD {item.TenTruong}Id int {BatBuocNhap} NULL
                                            ");
                                        break;
                                    case "date":
                                    case "datetime":
                                        ListInsertField.Add(@$"
                                            IF COL_LENGTH('{DatabaseName}.dbo.{_CreateObject.Name}','{item.TenTruong}') IS NOT NULL
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ALTER COLUMN {item.TenTruong} datetime2(0) {BatBuocNhap} NULL
                                            ELSE 
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ADD {item.TenTruong} datetime2(0) {BatBuocNhap} NULL
                                            ");
                                        break;
                                    case "bolean":
                                        ListInsertField.Add(@$"
                                            IF COL_LENGTH('{DatabaseName}.dbo.{_CreateObject.Name}','{item.TenTruong}') IS NOT NULL
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ALTER COLUMN {item.TenTruong} bit {BatBuocNhap} NULL
                                            ELSE 
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ADD {item.TenTruong} bit {BatBuocNhap} NULL
                                            ");

                                        break;
                                    case "Double":
                                        ListInsertField.Add(@$"
                                            IF COL_LENGTH('{DatabaseName}.dbo.{_CreateObject.Name}','{item.TenTruong}') IS NOT NULL
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ALTER COLUMN {item.TenTruong} bigint {BatBuocNhap} NULL
                                            ELSE 
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ADD {item.TenTruong} bigint {BatBuocNhap} NULL
                                            ");

                                        break;
                                    case "ParentId":
                                        ListInsertField.Add(@$"
                                            IF COL_LENGTH('{DatabaseName}.dbo.{_CreateObject.Name}','ParentId') IS NOT NULL
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ALTER COLUMN ParentId int {BatBuocNhap} NULL
                                            ELSE 
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ADD ParentId int {BatBuocNhap} NULL
                                            ");
                                        break;
                                    case "Int":
                                        ListInsertField.Add(@$"
                                            IF COL_LENGTH('{DatabaseName}.dbo.{_CreateObject.Name}','{item.TenTruong}') IS NOT NULL
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ALTER COLUMN {item.TenTruong} int {BatBuocNhap} NULL
                                            ELSE 
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ADD {item.TenTruong} int {BatBuocNhap} NULL
                                            ");
                                        break;
                                    case "image":
                                        ListInsertField.Add(@$"
                                            IF COL_LENGTH('{DatabaseName}.dbo.{_CreateObject.Name}','{item.TenTruong}Path') IS NOT NULL
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ALTER COLUMN {item.TenTruong}Path nvarchar(200) {BatBuocNhap} NULL
                                            ELSE 
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ADD {item.TenTruong}Path nvarchar(200) {BatBuocNhap} NULL
                                            ");

                                        break;
                                    case "Float":
                                        ListInsertField.Add(@$"
                                            IF COL_LENGTH('{DatabaseName}.dbo.{_CreateObject.Name}','{item.TenTruong}') IS NOT NULL
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ALTER COLUMN {item.TenTruong} bigint {BatBuocNhap} NULL
                                            ELSE 
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ADD {item.TenTruong} bigint {BatBuocNhap} NULL
                                            ");
                                        break;
                                    case "table":
                                        await CreateTable(_CreateObject, _ObjectField.TenBang);
                                        await InsertSubView(_CreateObject, _ObjectField.Id, item);
                                        break;
                                    default:
                                        ListInsertField.Add(@$"
                                            IF COL_LENGTH('{DatabaseName}.dbo.{_CreateObject.Name}','{item.TenTruong}') IS NOT NULL
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ALTER COLUMN {item.TenTruong} nvarchar({item.DoLon}) {BatBuocNhap} NULL
                                            ELSE 
                                                ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} ADD {item.TenTruong} nvarchar({item.DoLon}) {BatBuocNhap} NULL
                                            ");
                                        break;
                                }

                            }
                            var DisplayFieldOfTableList = _context.DisplayFieldOfTable.Where(item => item.ObjectFiledId == _ObjectField.Id);
                            foreach (var _DisplayFieldOfTable in DisplayFieldOfTableList)
                            {
                                _context.DisplayFieldOfTable.Remove(_DisplayFieldOfTable);
                            }
                            if (item.ListFieds != null) foreach (var Field in item.ListFieds)
                                {
                                    if (Field.Selected)
                                    {
                                        DisplayFieldOfTable _DisplayFieldOfTable = new();
                                        _DisplayFieldOfTable.CreatedDate = _CreateObject.CreatedDate;
                                        _DisplayFieldOfTable.CreatedBy = _CreateObject.CreatedBy;
                                        _DisplayFieldOfTable.ModifiedDate = DateTime.Now;
                                        _DisplayFieldOfTable.ModifiedBy = _UserName;
                                        _DisplayFieldOfTable.TenTruong = Field.TenTruong;
                                        _DisplayFieldOfTable.ObjectFiledId = _ObjectField.Id;
                                        _DisplayFieldOfTable.MoTa = Field.TenTruong;
                                        _context.Add(_DisplayFieldOfTable);
                                        await _context.SaveChangesAsync();
                                    }
                                }




                        }
                        if (ListUpdateField.Count() > 0)
                        {
                            using (var command = _context.Database.GetDbConnection().CreateCommand())
                            {
                                command.CommandText = @$"
                                {String.Join(";\n", ListUpdateField)}
                            ";
                                command.CommandType = CommandType.Text;
                                _context.Database.OpenConnection();
                                command.ExecuteNonQuery();
                            }
                        }
                        if (ListInsertField.Count() > 0)
                        {
                            using (var command = _context.Database.GetDbConnection().CreateCommand())
                            {
                                command.CommandText = @$"
                                {String.Join(";\n", ListInsertField)}
                            ";
                                command.CommandType = CommandType.Text;
                                _context.Database.OpenConnection();
                                command.ExecuteNonQuery();
                            }
                        }
                        List<string> ListDeleteField = new List<string>();
                        if (vm.itemsNeedDelete != null) foreach (var Id in vm.itemsNeedDelete)
                            {
                                ObjectField _ObjectField = await _context.ObjectField.FindAsync(Id);
                                if (_ObjectField.KieuDuLieu == "table")
                                {
                                    var TableName = _ObjectField.TenBang.Trim();
                                    ListDeleteField.Add(@$"DROP TABLE IF EXISTS[{DatabaseName}].[dbo].[{TableName}]");
                                }
                                else
                                {
                                    ListDeleteField.Add($"ALTER TABLE {DatabaseName}.dbo.{_CreateObject.Name} DROP COLUMN  IF EXISTS {_ObjectField.TenTruong}");
                                }

                                var DisplayFieldOfTableList = _context.DisplayFieldOfTable.Where(item => item.ObjectFiledId == _ObjectField.Id);
                                foreach (var DisplayFieldOfTable in DisplayFieldOfTableList)
                                {
                                    _context.DisplayFieldOfTable.Remove(DisplayFieldOfTable);
                                }
                                _context.ObjectField.Remove(_ObjectField);
                                await _context.SaveChangesAsync();
                            }
                        if (ListDeleteField.Count() > 0)
                        {
                            using (var command = _context.Database.GetDbConnection().CreateCommand())
                            {
                                command.CommandText = @$"
                                {String.Join(";\n", ListDeleteField)}
                            ";
                                command.CommandType = CommandType.Text;
                                _context.Database.OpenConnection();
                                command.ExecuteNonQuery();
                            }
                        }

                        string Name = MakeObject(ProjectPath, _CreateObject, Utility.RemoveUnicode(_CreateObject.Name), ObjectFields, ViewSource);
                        _JsonResultViewModel.AlertMessage = "Cập nhật view thành công. ID: " + _CreateObject.Id;
                        _JsonResultViewModel.IsSuccess = true;
                        return new JsonResult(_JsonResultViewModel);
                    }
                    else
                    {
                        var recordCount = _context.CreateObject.Count(item => item.Name == vm.Name);

                        if (recordCount > 0)
                        {
                            _JsonResultViewModel.AlertMessage = "View này đã tồn tại trong hệ thống, bạn chỉ có thể sửa, không được thêm mới";
                            _JsonResultViewModel.IsSuccess = false;
                            return new JsonResult(_JsonResultViewModel);
                        }
                        _CreateObject = vm;
                        _CreateObject.CreatedDate = DateTime.Now;
                        _CreateObject.ModifiedDate = DateTime.Now;
                        _CreateObject.CreatedBy = _UserName;
                        _CreateObject.ModifiedBy = _UserName;
                        _context.Add(_CreateObject);
                        await _context.SaveChangesAsync();
                        List<ObjectFieldCRUDViewModel> ObjectFields = vm.ObjectFields;
                        List<string> ListInsertField = new List<string>();
                        foreach (var item in ObjectFields)
                        {
                            ObjectField _ObjectField = new();
                            item.CreateObjectId = _CreateObject.Id;
                            item.CreatedDate = _CreateObject.CreatedDate;
                            item.CreatedBy = _CreateObject.CreatedBy;
                            item.ModifiedDate = DateTime.Now;
                            item.ModifiedBy = _UserName;
                            if (item.KieuDuLieu == "dropdown")
                            {
                                item.TreeView = item.ListFieds.Where(field => field.TenTruong == "ParentId").Count() >= 1;
                            }
                            _ObjectField = item;
                            _context.Add(_ObjectField);
                            await _context.SaveChangesAsync();
                            string BatBuocNhap = item.BatBuocNhap ? "NOT" : "";
                            switch (item.KieuDuLieu)
                            {
                                case "dropdown":
                                    ListInsertField.Add($"[{item.TenTruong}Id] [int] {BatBuocNhap} NULL");
                                    break;
                                case "date":
                                case "datetime":
                                    ListInsertField.Add($"[{item.TenTruong}] [datetime2](7) {BatBuocNhap} NULL,");
                                    break;
                                case "table":
                                    await CreateTable(_CreateObject, _ObjectField.TenBang);
                                    await InsertSubView(_CreateObject, _ObjectField.Id, item);
                                    break;
                                case "bolean":
                                    ListInsertField.Add($"[{item.TenTruong}] [bit] {BatBuocNhap} NULL");
                                    break;
                                case "image":
                                    ListInsertField.Add($"[{item.TenTruong}Path] [nvarchar](200) {BatBuocNhap} NULL");
                                    break;
                                case "ParentId":
                                    ListInsertField.Add($"[{item.TenTruong}] [int] NOT NULL");
                                    break;
                                default:
                                    ListInsertField.Add($"[{item.TenTruong}] [nvarchar]({item.DoLon}) {BatBuocNhap} NULL");
                                    break;
                            }

                            if (item.KieuDuLieu == "dropdown" && item.ListFieds != null) foreach (var Field in item.ListFieds)
                                {
                                    if (Field.Selected)
                                    {
                                        DisplayFieldOfTable _DisplayFieldOfTable = new();
                                        _DisplayFieldOfTable.ObjectFiledId = _ObjectField.Id;
                                        _DisplayFieldOfTable.CreatedDate = _CreateObject.CreatedDate;
                                        _DisplayFieldOfTable.CreatedBy = _CreateObject.CreatedBy;
                                        _DisplayFieldOfTable.ModifiedDate = DateTime.Now;
                                        _DisplayFieldOfTable.ModifiedBy = _UserName;
                                        _DisplayFieldOfTable.TenTruong = Field.TenTruong;
                                        _DisplayFieldOfTable.MoTa = Field.TenTruong;
                                        _context.Add(_DisplayFieldOfTable);
                                        await _context.SaveChangesAsync();
                                    }
                                }

                        }
                        using (var command = _context.Database.GetDbConnection().CreateCommand())
                        {
                            command.CommandText = @$"
                            CREATE TABLE [dbo].[{_CreateObject.Name}](
	                            {String.Join(",\n", ListInsertField)},
	                            [CreatedDate] [datetime2](7) NOT NULL,
	                            [ModifiedDate] [datetime2](7) NOT NULL,
	                            [CreatedBy] [nvarchar](max) NULL,
	                            [ModifiedBy] [nvarchar](max) NULL,
	                            [Cancelled] [bit] NOT NULL,
	                            [Id] [int] IDENTITY(2,1) NOT NULL PRIMARY KEY
                            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                            ";
                            command.CommandType = CommandType.Text;
                            _context.Database.OpenConnection();
                            command.ExecuteNonQuery();
                        }


                        string Name = MakeObject(ProjectPath, _CreateObject, Utility.RemoveUnicode(_CreateObject.Name), ObjectFields, ViewSource);
                        _JsonResultViewModel.AlertMessage = "Tạo view thành công. ID: " + _CreateObject.Id;
                        _JsonResultViewModel.IsSuccess = true;
                        return new JsonResult(_JsonResultViewModel);
                    }
                }
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
                _JsonResultViewModel.AlertMessage = "Tạo View không thành công:" + messages;
                _JsonResultViewModel.IsSuccess = false;
                return new JsonResult(_JsonResultViewModel);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _JsonResultViewModel.AlertMessage = "Tạo View không thành công:" + ex.Message;
                _JsonResultViewModel.IsSuccess = false;
                return new JsonResult(_JsonResultViewModel);

            }
        }

        public async Task CreateTable(CreateObject _CreateObject, string Table)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @$"
                            CREATE TABLE [dbo].[{Table}](
                                [Id] [int] IDENTITY(2,1) NOT NULL PRIMARY KEY,
                                [{_CreateObject.Name}Id] [int] NOT NULL,
	                            [CreatedDate] [datetime2](7) NOT NULL,
	                            [ModifiedDate] [datetime2](7) NOT NULL,
	                            [CreatedBy] [nvarchar](max) NULL,
	                            [ModifiedBy] [nvarchar](max) NULL,
	                            [Cancelled] [bit] NOT NULL
                            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                            ";
                command.CommandType = CommandType.Text;
                _context.Database.OpenConnection();
                command.ExecuteNonQuery();
            }
        }
        public async Task InsertSubView(CreateObject _CreateObject, int ObjectFieldId, ObjectFieldCRUDViewModel objectFieldCRUDViewModel)
        {
            var TenBang = objectFieldCRUDViewModel.TenBang;
            var DatabaseName = _iCommon.GetDatabaseName();
            List<string> ListInsertField = new List<string>();
            string _UserName = HttpContext.User.Identity.Name;
            var sub_items = objectFieldCRUDViewModel.sub_items.Where(item => item.Id == 0).ToList();

            if (sub_items.Count() > 0)
            {
                foreach (var subItem in sub_items)
                {
                    SubViewField _SubView = subItem;
                    _SubView.OjbectFieldId = ObjectFieldId;
                    _SubView.CreatedDate = DateTime.Now;
                    _SubView.ModifiedDate = DateTime.Now;
                    _SubView.CreatedBy = _UserName;
                    _SubView.ModifiedBy = _UserName;
                    _context.Add(_SubView);
                    await _context.SaveChangesAsync();
                    string BatBuocNhap = subItem.BatBuocNhap ? "NOT" : "";
                    switch (subItem.KieuDuLieu)
                    {
                        case "dropdown":
                            if (subItem.ListFieds != null) foreach (var Field in subItem.ListFieds)
                                {
                                    if (Field.Selected)
                                    {
                                        DisplaySubViewFieldOfTable _DisplaySubViewFieldOfTable = new();
                                        _DisplaySubViewFieldOfTable.ObjectSubViewFiledId = _SubView.Id;
                                        _DisplaySubViewFieldOfTable.CreatedDate = _SubView.CreatedDate;
                                        _DisplaySubViewFieldOfTable.CreatedBy = _SubView.CreatedBy;
                                        _DisplaySubViewFieldOfTable.ModifiedDate = DateTime.Now;
                                        _DisplaySubViewFieldOfTable.ModifiedBy = _UserName;
                                        _DisplaySubViewFieldOfTable.TenTruong = Field.TenTruong;
                                        _DisplaySubViewFieldOfTable.MoTa = Field.TenTruong;
                                        _context.Add(_DisplaySubViewFieldOfTable);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            ListInsertField.Add(@$"
                                IF COL_LENGTH('{DatabaseName}.dbo.{TenBang}','{subItem.TenTruong}') IS NOT NULL
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ALTER COLUMN {subItem.TenTruong}Id int {BatBuocNhap} NULL
                                ELSE 
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ADD {subItem.TenTruong}Id int {BatBuocNhap} NULL
                                ");


                            break;
                        case "date":
                        case "datetime":
                            ListInsertField.Add(@$"
                                IF COL_LENGTH('{DatabaseName}.dbo.{TenBang}','{subItem.TenTruong}') IS NOT NULL
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ALTER COLUMN {subItem.TenTruong} datetime2(0) {BatBuocNhap} NULL
                                ELSE 
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ADD {subItem.TenTruong} datetime2(0) {BatBuocNhap} NULL
                                ");

                            break;
                        case "bolean":
                            ListInsertField.Add(@$"
                                IF COL_LENGTH('{DatabaseName}.dbo.{TenBang}','{subItem.TenTruong}') IS NOT NULL
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ALTER COLUMN {subItem.TenTruong} bit {BatBuocNhap} NULL
                                ELSE 
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ADD {subItem.TenTruong} bit {BatBuocNhap} NULL
                                ");
                            break;
                        case "Double":
                            ListInsertField.Add(@$"
                                IF COL_LENGTH('{DatabaseName}.dbo.{TenBang}','{subItem.TenTruong}') IS NOT NULL
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ALTER COLUMN {subItem.TenTruong} bigint {BatBuocNhap} NULL
                                ELSE 
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ADD {subItem.TenTruong} bigint {BatBuocNhap} NULL
                                ");
                            break;
                        case "ParentId":
                            ListInsertField.Add(@$"
                                IF COL_LENGTH('{DatabaseName}.dbo.{TenBang}','{subItem.TenTruong}') IS NOT NULL
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ALTER COLUMN {subItem.TenTruong} int {BatBuocNhap} NULL
                                ELSE 
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ADD {subItem.TenTruong} int {BatBuocNhap} NULL
                                ");
                            break;
                        case "Int":
                            ListInsertField.Add(@$"
                                IF COL_LENGTH('{DatabaseName}.dbo.{TenBang}','{subItem.TenTruong}') IS NOT NULL
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ALTER COLUMN {subItem.TenTruong} int {BatBuocNhap} NULL
                                ELSE 
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ADD {subItem.TenTruong} int {BatBuocNhap} NULL,CONSTRAINT {subItem.TenTruong} DEFAULT(0) FOR {subItem.TenTruong}
                                ");
                            break;
                        case "Float":
                            ListInsertField.Add(@$"
                                IF COL_LENGTH('{DatabaseName}.dbo.{TenBang}','{subItem.TenTruong}') IS NOT NULL
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ALTER COLUMN {subItem.TenTruong} float {BatBuocNhap} NULL
                                ELSE 
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ADD {subItem.TenTruong} float {BatBuocNhap} NULL
                                ");

                            break;
                        default:
                            ListInsertField.Add(@$"
                                IF COL_LENGTH('{DatabaseName}.dbo.{TenBang}','{subItem.TenTruong}') IS NOT NULL
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ALTER COLUMN {subItem.TenTruong} nvarchar({subItem.DoLon}) {BatBuocNhap} NULL
                                ELSE 
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ADD {subItem.TenTruong} nvarchar({subItem.DoLon}) {BatBuocNhap} NULL
                                ");
                            break;
                    }

                }
            }
            if (ListInsertField.Count() > 0)
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = @$"
                                {String.Join(";\n", ListInsertField)}
                            ";
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task UpdateSubView(int ObjectFieldId, ObjectFieldCRUDViewModel objectFieldCRUDViewModel)
        {
            var TenBang = objectFieldCRUDViewModel.TenBang.Trim();
            var DatabaseName = _iCommon.GetDatabaseName();
            List<string> ListUpdateField = new List<string>();
            string _UserName = HttpContext.User.Identity.Name;
            var sub_items = objectFieldCRUDViewModel.sub_items.Where(item => item.Id != 0).ToList();
            if (sub_items.Count() > 0)
            {
                foreach (var subItem in sub_items)
                {
                    var _SubView = new SubViewField();
                    _SubView.OjbectFieldId = ObjectFieldId;
                    _SubView.CreatedDate = DateTime.Now;
                    _SubView.ModifiedDate = DateTime.Now;
                    _SubView.CreatedBy = _UserName;
                    _SubView.ModifiedBy = _UserName;
                    _context.Entry(_SubView).CurrentValues.SetValues(subItem);
                    await _context.SaveChangesAsync();
                    string BatBuocNhap = subItem.BatBuocNhap ? "NOT" : "";
                    switch (subItem.KieuDuLieu)
                    {
                        case "dropdown":
                            ListUpdateField.Add(@$"

                                IF COL_LENGTH('{DatabaseName}.dbo.{TenBang}','{subItem.TenTruong}') IS NOT NULL
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ALTER COLUMN  {subItem.TenTruong}Id int {BatBuocNhap} NULL

                            ");
                            var DisplaySubViewFieldOfTableList = _context.DisplaySubViewFieldOfTable.Where(item => item.ObjectSubViewFiledId == subItem.Id);
                            foreach (var _DisplaySubViewFieldOfTable in DisplaySubViewFieldOfTableList)
                            {
                                _context.DisplaySubViewFieldOfTable.Remove(_DisplaySubViewFieldOfTable);
                            }
                            if (subItem.ListFieds != null) foreach (var Field in subItem.ListFieds)
                                {
                                    if (Field.Selected)
                                    {
                                        DisplaySubViewFieldOfTable _DisplaySubViewFieldOfTable = new();
                                        _DisplaySubViewFieldOfTable.ObjectSubViewFiledId = _SubView.Id;
                                        _DisplaySubViewFieldOfTable.CreatedDate = _SubView.CreatedDate;
                                        _DisplaySubViewFieldOfTable.CreatedBy = _SubView.CreatedBy;
                                        _DisplaySubViewFieldOfTable.ModifiedDate = DateTime.Now;
                                        _DisplaySubViewFieldOfTable.ModifiedBy = _UserName;
                                        _DisplaySubViewFieldOfTable.TenTruong = Field.TenTruong;
                                        _DisplaySubViewFieldOfTable.MoTa = Field.TenTruong;
                                        _context.Add(_DisplaySubViewFieldOfTable);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            break;
                        case "date":
                        case "datetime":

                            ListUpdateField.Add(@$"
                                IF COL_LENGTH('{DatabaseName}.dbo.{TenBang}','{subItem.TenTruong}') IS NOT NULL
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ALTER COLUMN {subItem.TenTruong} datetime2(0) {BatBuocNhap} NULL
                                ELSE 
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ADD {subItem.TenTruong} datetime2(0) {BatBuocNhap} NULL
                                ");
                            break;
                        case "bolean":
                            ListUpdateField.Add(@$"
                                IF COL_LENGTH('{DatabaseName}.dbo.{TenBang}','{subItem.TenTruong}') IS NOT NULL
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ALTER COLUMN {subItem.TenTruong} bit {BatBuocNhap} NULL
                                ELSE 
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ADD {subItem.TenTruong} bit {BatBuocNhap} NULL
                                ");

                            break;
                        case "Int":
                            ListUpdateField.Add(@$"
                                IF COL_LENGTH('{DatabaseName}.dbo.{TenBang}','{subItem.TenTruong}') IS NOT NULL
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ALTER COLUMN {subItem.TenTruong} int {BatBuocNhap} NULL
                                ELSE 
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ADD {subItem.TenTruong} int {BatBuocNhap} NULL ,CONSTRAINT {subItem.TenTruong} DEFAULT(0) FOR {subItem.TenTruong}
                                ");

                            break;
                        case "image":
                            ListUpdateField.Add(@$"
                                IF COL_LENGTH('{DatabaseName}.dbo.{TenBang}','{subItem.TenTruong}Path') IS NOT NULL
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ALTER COLUMN {subItem.TenTruong}Path nvarchar(200) {BatBuocNhap} NULL
                                ELSE 
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ADD {subItem.TenTruong}Path nvarchar(200) {BatBuocNhap} NULL
                                ");

                            break;
                        default:
                            ListUpdateField.Add(@$"
                                IF COL_LENGTH('{DatabaseName}.dbo.{TenBang}','{subItem.TenTruong}') IS NOT NULL
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ALTER COLUMN {subItem.TenTruong} nvarchar({subItem.DoLon}) {BatBuocNhap} NULL
                                ELSE 
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} ADD {subItem.TenTruong} nvarchar({subItem.DoLon}) {BatBuocNhap} NULL
                                ");
                            break;
                    }

                }
            }
            if (ListUpdateField.Count() > 0)
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = @$"
                                {String.Join("\n", ListUpdateField)}
                            ";
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();
                    command.ExecuteNonQuery();
                }
            }
        }
        public async Task DeleteSubViewFieldNeedDelete(int ObjectFieldId, ObjectFieldCRUDViewModel objectFieldCRUDViewModel)
        {
            var TenBang = objectFieldCRUDViewModel.TenBang;
            var DatabaseName = _iCommon.GetDatabaseName();

            List<string> ListUpdateField = new List<string>();
            List<string> ListDeleteSubViewField = new List<string>();
            if (objectFieldCRUDViewModel.subItemsNeedDelete != null) foreach (var Id in objectFieldCRUDViewModel.subItemsNeedDelete)
                {
                    SubViewField _SubViewField = await _context.SubViewField.FindAsync(Id);
                    ListDeleteSubViewField.Add(@$"
                                IF COL_LENGTH('{DatabaseName}.dbo.{TenBang}','{_SubViewField.TenTruong}') IS NOT NULL
                                    ALTER TABLE {DatabaseName}.dbo.{TenBang} DROP COLUMN {_SubViewField.TenTruong}
                                ");
                    var DisplaySubViewFieldOfTableList = _context.DisplaySubViewFieldOfTable.Where(item => item.ObjectSubViewFiledId == _SubViewField.Id);
                    foreach (var DisplaySubViewFieldOfTable in DisplaySubViewFieldOfTableList)
                    {
                        _context.DisplaySubViewFieldOfTable.Remove(DisplaySubViewFieldOfTable);
                    }

                    _context.SubViewField.Remove(_SubViewField);
                    await _context.SaveChangesAsync();
                }
            if (ListDeleteSubViewField.Count() > 0)
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = @$"
                                {String.Join(";\n", ListDeleteSubViewField)}
                            ";
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();
                    command.ExecuteNonQuery();
                }
            }



        }
        [HttpGet]
        public async Task<JsonResult> GetFielsTable(string tableName)
        {
            JsonResultViewModel _JsonResultViewModel = new();
            try
            {
                List<String> ListFields = new List<String>();
                ListFields = _iCommon.LoadFieldsByTableName(tableName);
                _JsonResultViewModel.ModelObject = ListFields;
                _JsonResultViewModel.AlertMessage = "";
                _JsonResultViewModel.IsSuccess = true;
                return new JsonResult(_JsonResultViewModel);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _JsonResultViewModel.AlertMessage = "Đã có lỗi";
                _JsonResultViewModel.IsSuccess = false;
                return new JsonResult(_JsonResultViewModel);

            }
        }
        public string MakeObject(string ProjectPath, CreateObject _CreateObject, string Name, List<ObjectFieldCRUDViewModel> ObjectFields, string ViewSource = "AssetCategories")
        {
            if (Name.Any(Char.IsWhiteSpace))
            {
                Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Name.ToLower());
                Name = String.Concat(Name.Where(c => !Char.IsWhiteSpace(c)));
            }

            try
            {
                WriteJSScript(ProjectPath, _CreateObject, Name, ObjectFields, ViewSource);
                WriteLesscript(ProjectPath, _CreateObject, Name, ObjectFields, ViewSource);
                WriteController(ProjectPath, _CreateObject, Name, ObjectFields, ViewSource);
                WriteModel(ProjectPath, _CreateObject, Name, ObjectFields, ViewSource);
                WriteSubModel(ProjectPath, _CreateObject, Name, ObjectFields, ViewSource);
                WriteSubDbContent(ProjectPath, _CreateObject, Name, ObjectFields, ViewSource);
                WriteViewModel(ProjectPath, _CreateObject, Name, ObjectFields, ViewSource);
                WriteSubViewModel(ProjectPath, _CreateObject, Name, ObjectFields, ViewSource);
                WriteViews(ProjectPath, _CreateObject, Name, ObjectFields, ViewSource);
                WriteDbContent(ProjectPath, _CreateObject, Name, ObjectFields, ViewSource);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Name;

        }
        public void WriteModel(string ProjectPath, CreateObject _CreateObject, string Name, List<ObjectFieldCRUDViewModel> ObjectFields, string ViewSource = "AssetCategories")
        {
            string new_model_path = Path.Combine(ProjectPath, "Models\\" + Name + ".cs");
            System.IO.File.Copy(
               Path.Combine(ProjectPath, "Models\\" + ViewSource + ".cs"),
               new_model_path,
               true);
            string new_model_content = System.IO.File.ReadAllText(new_model_path);
            new_model_content = new_model_content.Replace(ViewSource, Name);


            //insert new fieds of model
            List<string> list_new_field_of_model = new List<string>();
            foreach (var item in ObjectFields)
            {
                string TenTruong = item.TenTruong;
                string KieuDuLieu = "";
                if (item.KieuDuLieu != "dropdown")
                {
                    if (TenTruong.Any(Char.IsWhiteSpace))
                    {
                        TenTruong = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.TenTruong.ToLower());
                        TenTruong = Utility.RemoveUnicode(TenTruong);
                        TenTruong = String.Concat(TenTruong.Where(c => !Char.IsWhiteSpace(c)));
                    }
                    KieuDuLieu = "";
                }
                string BatBuocNhap = item.BatBuocNhap ? "" : "?";
                switch (item.KieuDuLieu)
                {
                    case "dropdown":
                        KieuDuLieu = "int";
                        TenTruong = $"{TenTruong}Id";
                        list_new_field_of_model.Add($"public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}");
                        break;
                    case "date":
                    case "datetime":
                        KieuDuLieu = "DateTime";
                        list_new_field_of_model.Add($"public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}");
                        break;
                    case "bolean":
                        KieuDuLieu = "bool";
                        list_new_field_of_model.Add($"public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}");
                        break;
                    case "ParentId":
                        KieuDuLieu = "int";
                        list_new_field_of_model.Add($"public int ParentId {{ get; set; }}");
                        break;
                    case "Double":
                        KieuDuLieu = "double";
                        list_new_field_of_model.Add($"public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}");
                        break;
                    case "table":
                        //nothing
                        break;
                    case "Float":
                        KieuDuLieu = "float";
                        list_new_field_of_model.Add($"public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}");
                        break;
                    case "Int":
                        KieuDuLieu = "int";
                        list_new_field_of_model.Add($"public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}");
                        break;
                    case "image":
                        KieuDuLieu = "string";
                        list_new_field_of_model.Add($"public {KieuDuLieu}? {TenTruong}Path {{ get; set; }}");
                        break;
                    default:
                        KieuDuLieu = "string";
                        list_new_field_of_model.Add($"public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}");
                        break;
                }



            }

            string new_fields_of_model = String.Join("\r\n", list_new_field_of_model);

            new_model_content = new_model_content.Replace("//[INSERT_FIELD_MODEL]", new_fields_of_model);

            System.IO.File.WriteAllText(new_model_path, new_model_content);
        }
        public void WriteSubModel(string ProjectPath, CreateObject _CreateObject, string Name, List<ObjectFieldCRUDViewModel> ObjectFields, string ViewSource = "AssetCategories")
        {

            foreach (var item in ObjectFields)
            {
                if (item.KieuDuLieu == "table")
                {
                    item.TenBang = item.TenBang.Trim();
                    var sub_items = item.sub_items;
                    string new_model_path = Path.Combine(ProjectPath, "Models\\" + item.TenBang + ".cs");
                    System.IO.File.Copy(
                       Path.Combine(ProjectPath, "Models\\" + ViewSource + ".cs"),
                       new_model_path,
                       true);
                    string new_model_content = System.IO.File.ReadAllText(new_model_path);
                    new_model_content = new_model_content.Replace(ViewSource, item.TenBang);


                    //insert new fieds of model
                    List<string> list_new_field_of_model = new List<string>();
                    list_new_field_of_model.Add($"public int {Name}Id {{ get; set; }}");
                    foreach (var sub_item in sub_items)
                    {
                        string TenTruong = sub_item.TenTruong;
                        string KieuDuLieu = "";
                        if (sub_item.KieuDuLieu != "dropdown")
                        {
                            if (TenTruong.Any(Char.IsWhiteSpace))
                            {
                                TenTruong = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sub_item.TenTruong.ToLower());
                                TenTruong = Utility.RemoveUnicode(TenTruong);
                                TenTruong = String.Concat(TenTruong.Where(c => !Char.IsWhiteSpace(c)));
                            }
                            KieuDuLieu = "";
                        }
                        string BatBuocNhap = item.BatBuocNhap ? "" : "?";
                        switch (sub_item.KieuDuLieu)
                        {
                            case "dropdown":
                                KieuDuLieu = "int";
                                TenTruong = $"{TenTruong}Id";
                                list_new_field_of_model.Add($"public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}");
                                break;
                            case "date":
                            case "datetime":
                                KieuDuLieu = "DateTime";
                                list_new_field_of_model.Add($"public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}");
                                break;
                            case "bolean":
                                KieuDuLieu = "bool";
                                list_new_field_of_model.Add($"public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}");
                                break;
                            case "ParentId":
                                KieuDuLieu = "int";
                                list_new_field_of_model.Add($"public int ParentId {{ get; set; }}");
                                break;
                            case "Double":
                                KieuDuLieu = "double";
                                list_new_field_of_model.Add($"public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}");
                                break;
                            case "Float":
                                KieuDuLieu = "float";
                                list_new_field_of_model.Add($"public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}");
                                break;
                            case "image":
                                KieuDuLieu = "string";
                                list_new_field_of_model.Add($"public {KieuDuLieu}? {TenTruong}Path {{ get; set; }}");
                                break;
                            case "Int":
                                KieuDuLieu = "int";
                                list_new_field_of_model.Add($"public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}");
                                break;
                            default:
                                KieuDuLieu = "string";
                                list_new_field_of_model.Add($"public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}");
                                break;
                        }


                    }
                    string new_fields_of_model = String.Join("\r\n", list_new_field_of_model);
                    new_model_content = new_model_content.Replace("//[INSERT_FIELD_MODEL]", new_fields_of_model);

                    System.IO.File.WriteAllText(new_model_path, new_model_content);
                }


            }



        }
        public void WriteDbContent(string ProjectPath, CreateObject _CreateObject, string Name, List<ObjectFieldCRUDViewModel> ObjectFields, string ViewSource = "AssetCategories")
        {
            string DbContent = $"public DbSet<{Name}> {Name} {{ get; set; }}\r\n\t\t//[DBSET]";
            string ApplicationDbContextPath = Path.Combine(ProjectPath, "Data\\ApplicationDbContext.cs");
            string ApplicationDbContextContent = System.IO.File.ReadAllText(ApplicationDbContextPath);
            if (!ApplicationDbContextContent.Contains($"DbSet<{Name}>"))
            {
                ApplicationDbContextContent = ApplicationDbContextContent.Replace("//[DBSET]", DbContent);
                System.IO.File.WriteAllText(ApplicationDbContextPath, ApplicationDbContextContent);
            }
        }
        public void WriteSubDbContent(string ProjectPath, CreateObject _CreateObject, string Name, List<ObjectFieldCRUDViewModel> ObjectFields, string ViewSource = "AssetCategories")
        {
            foreach (var item in ObjectFields)
            {
                if (item.KieuDuLieu == "table")
                {
                    item.TenBang = item.TenBang.Trim();
                    string DbContent = $"public DbSet<{item.TenBang}> {item.TenBang} {{ get; set; }}\r\n\t\t//[DBSET]";
                    string ApplicationDbContextPath = Path.Combine(ProjectPath, "Data\\ApplicationDbContext.cs");
                    string ApplicationDbContextContent = System.IO.File.ReadAllText(ApplicationDbContextPath);
                    if (!ApplicationDbContextContent.Contains($"DbSet<{item.TenBang}>"))
                    {
                        ApplicationDbContextContent = ApplicationDbContextContent.Replace("//[DBSET]", DbContent);
                        System.IO.File.WriteAllText(ApplicationDbContextPath, ApplicationDbContextContent);
                    }
                }
            }

        }
        public void WriteViews(string ProjectPath, CreateObject _CreateObject, string Name, List<ObjectFieldCRUDViewModel> ObjectFields, string ViewSource = "AssetCategories")
        {
            string ViewPathDirectory = Path.Combine(ProjectPath, "Views\\" + Name);
            Directory.CreateDirectory(ViewPathDirectory);
            CopyFilesRecursively(Path.Combine(ProjectPath, "Views\\" + ViewSource), ViewPathDirectory);
            ChangeContentFolder(ViewPathDirectory, Name, ViewSource);


            //insert new fieds of table
            List<string> list_new_field_of_table = new List<string>();
            foreach (var item in ObjectFields)
            {
                if (item.KieuDuLieu != "table" && item.HienThiTrongBang)
                    list_new_field_of_table.Add($"<th>{item.Label}</th>");

            }
            string new_fields_of_table = String.Join("\n", list_new_field_of_table);
            string IndexCsHtmlPath = Path.Combine(ProjectPath, $"Views\\{Name}\\Index.cshtml");
            string IndexCsHtmlPathContent = System.IO.File.ReadAllText(IndexCsHtmlPath);
            IndexCsHtmlPathContent = IndexCsHtmlPathContent.Replace("@*//[INSERT_FIELD_SHOW_TABLE]*@", new_fields_of_table);
            System.IO.File.WriteAllText(IndexCsHtmlPath, IndexCsHtmlPathContent);

            string htmlRows = "";
            //insert new fieds of edit
            var MaxRow = ObjectFields.Max(field => field.RowIndex);
            for (var iRow = 0; iRow <= MaxRow; iRow++)
            {
                htmlRows += $"<div class=\"row\">";
                var columns = ObjectFields.Where(field => field.RowIndex == iRow);
                var MaxColumn = columns.Max(column => column.ColumnIndex);
                var ColumnClass = MaxColumn == 0 ? "12" : "6";
                for (var iColumn = 0; iColumn <= MaxColumn; iColumn++)
                {
                    htmlRows += $"<div class=\"col-{ColumnClass}\">";
                    var fields = ObjectFields.Where(field => field.RowIndex == iRow && field.ColumnIndex == iColumn).ToList();
                    foreach (var field in fields)
                    {

                        string TenTruong = field.TenTruong;
                        if (field.KieuDuLieu != "dropdown")
                        {
                            if (TenTruong.Any(Char.IsWhiteSpace))
                            {
                                TenTruong = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(TenTruong.ToLower());
                                TenTruong = Utility.RemoveUnicode(TenTruong);
                                TenTruong = String.Concat(TenTruong.Where(c => !Char.IsWhiteSpace(c)));
                            }
                        }
                        switch (field.KieuDuLieu)
                        {
                            case "dropdown":
                                string TenTruongNoId = TenTruong;
                                var TenTruongId = $"{TenTruong}Id";

                                htmlRows += @$"
                                                    <div class=""form-group row"" >
                                                        <label asp-for= ""{TenTruongId}"" class=""col-sm-3 col-form-label""></label>
                                                        <div class=""col-sm-9"">
                                                            <select asp-for=""{TenTruongId}"" asp-items=""@ViewBag._Load{TenTruong}""
                                                                    id=""{TenTruongId}"" class=""form-control"" style=""width:100%;"">
                                                                <option disabled selected>--- SELECT ---</option>
                                                            </select>
                                                            <span asp-validation-for=""{TenTruongId}"" class=""text-danger""></span>
                                                        </div>
                                                    </div>";
                                break;
                            case "ParentId":
                                var ViewBagLoad = TenTruong;
                                if (field.TenTruong.Equals("ParentId"))
                                {
                                    ViewBagLoad = _CreateObject.Name;
                                }
                                htmlRows += @$"
                                                    <div class=""form-group row"" >
                                                        <label asp-for= ""{TenTruong}"" class=""col-sm-3 col-form-label""></label>
                                                        <div class=""col-sm-9"">
                                                            <select asp-for=""{TenTruong}"" asp-items=""@ViewBag._Load{ViewBagLoad}""
                                                                    id=""{TenTruong}"" class=""form-control"" style=""width:100%;"">
                                                                <option disabled selected>--- SELECT ---</option>
                                                            </select>
                                                            <span asp-validation-for=""{TenTruong}"" class=""text-danger""></span>
                                                        </div>
                                                    </div>";
                                break;
                            case "image":
                                htmlRows += @$"
                                                <div class=""form-group"">
                                                    <label asp-for=""{TenTruong}"" class=""col-sm-12 control-label""></label>
                                                    <div class=""col-sm-12"">
                                                        <div class=""image-thumb"" id=""{TenTruong}"" @@click=""$refs.{TenTruong}.click()"">
                                                            <img class=""image-thumb-upload"" src=""/images/DefaultAsset/upload.png"" />
                                                        </div>
                                                        <input asp-for=""{TenTruong}"" type=""file"" ref=""{TenTruong}"" accept=""image/*"" v-on:change=""fileSelected($event,'{TenTruong}')"" enctype=""multipart/form-data"" style=""display:none"" />
                                                    </div>

                                                    <span asp-validation-for=""{TenTruong}"" class=""text-danger""></span>
                                                </div>
                                                ";
                                break;
                            case "table":
                                htmlRows += RenderTable(field);
                                break;
                            default:
                                htmlRows += @$"
                                                    <div class=""form-group"">
                                                        <label asp-for= ""{TenTruong}"" class=""control-label""></label>
                                                        <input asp-for=""{TenTruong}"" class=""form-control"" />
                                                        <span asp-validation-for=""{TenTruong}"" class=""text-danger""></span>
                                                    </div>";
                                break;
                        }
                    }
                    htmlRows += $"</div>";
                }
                htmlRows += $"</div>";

            }

            string EditCsHtmlPath = Path.Combine(ProjectPath, $"Views\\{Name}\\_AddEdit.cshtml");
            string EditCsHtmlPathContent = System.IO.File.ReadAllText(EditCsHtmlPath);
            EditCsHtmlPathContent = EditCsHtmlPathContent.Replace("@*//[INSERT_FIELD_SHOW_TABLE]*@", htmlRows);
            List<string> InsertFieldJsList = new List<string>();
            List<string> InsertMethodThemXoaList = new List<string>();
            var InsertFieldJs = "";
            var InsertMethodThemXoaListString = "";
            foreach (var item in ObjectFields)
            {
                switch (item.KieuDuLieu)
                {
                    case "dropdown":
                        InsertFieldJsList.Add(@$"{item.TenTruong}Id:@Html.Raw(@Model.{item.TenTruong}Id)");
                        break;
                    case "date":
                    case "datetime":
                        InsertFieldJsList.Add(@$"{item.TenTruong}:new Date(@Html.Raw(@Model.{item.TenTruong})) ");
                        break;
                    case "bolean":
                    case "Double":
                    case "Int":
                    case "Float":
                        InsertFieldJsList.Add(@$"{item.TenTruong}:@Html.Raw(@Model.{item.TenTruong})");

                        break;
                    case "table":
                        InsertFieldJsList.Add(@$"Items{item.TenTruong}:@Html.Raw(ViewBag.JsonItems{item.TenTruong})");
                        InsertFieldJsList.Add(@$"ListItems{item.TenTruong}NeedDelete:[]");
                        InsertMethodThemXoaList.Add(@$"
                                AddRow{item.TenTruong}: function (event) {{
                                    this.Items{item.TenTruong}.push({{
                                         RamdomKey: this.makeid(6),
                                    }});
                                }},
                                RemoveRow{item.TenTruong}: function (event,index) {{
                                    if (index > -1 && this.Items{item.TenTruong}.length > 1) {{ 
                                        this.ListItems{item.TenTruong}NeedDelete.push(this.Items{item.TenTruong}[index].Id);
                                        this.Items{item.TenTruong}.splice(index, 1);
                                    }}

                                }}
                            ");
                        var sub_items = LoadSubViewCRUDViewModelByOjbectFieldId(item.Id).ToList();
                        foreach (var sub_item in sub_items)
                        {
                            switch (sub_item.KieuDuLieu)
                            {
                                case "dropdown":
                                    InsertFieldJsList.Add(@$"Items{sub_item.TenTruong}SubView:@Html.Raw(ViewBag.JsonItems{sub_item.TenTruong}SubView)");
                                    break;
                                case "image":
                                    break;
                                case "date":
                                case "datetime":
                                case "bolean":
                                case "Double":
                                case "Int":
                                case "Float":
                                case "string":
                                    break;
                            }
                        }
                        break;
                    default:
                        InsertFieldJsList.Add(@$"{item.TenTruong}:""@Html.Raw(@Model.{item.TenTruong})""");
                        break;
                }
            }

            if (InsertFieldJsList.Count() > 0)
            {
                InsertFieldJs = String.Join(",\n", InsertFieldJsList) + ",";
            }
            if (InsertMethodThemXoaList.Count() > 0)
            {
                InsertMethodThemXoaListString = String.Join(",\n", InsertMethodThemXoaList) + ",";
            }
            EditCsHtmlPathContent = EditCsHtmlPathContent.Replace("@*//[INSERT_FIELD_JS]*@", InsertFieldJs);
            EditCsHtmlPathContent = EditCsHtmlPathContent.Replace("@*//[INSERT_FUNCTION_JS]*@", InsertMethodThemXoaListString);
            System.IO.File.WriteAllText(EditCsHtmlPath, EditCsHtmlPathContent);
        }
        public void WriteJSScript(string ProjectPath, CreateObject _CreateObject, string Name, List<ObjectFieldCRUDViewModel> ObjectFields, string ViewSource = "AssetCategories")
        {
            string js_path = Path.Combine(ProjectPath, "wwwroot\\js", Name);
            Directory.CreateDirectory(js_path);

            string new_file_js_CRUD = Path.Combine(ProjectPath, "wwwroot\\js\\" + Name + "\\" + Name + "_CRUD.js");
            System.IO.File.Copy(
                Path.Combine(ProjectPath, "wwwroot\\js\\" + ViewSource + "\\" + ViewSource + "_CRUD.js"),
                new_file_js_CRUD,
                true);

            string new_file_js_CRUD_content = System.IO.File.ReadAllText(new_file_js_CRUD);
            new_file_js_CRUD_content = new_file_js_CRUD_content.Replace(ViewSource, Name);


            List<string> InsertFieldPostList = new List<string>();
            var InsertFieldPostListString = "";
            foreach (var item in ObjectFields)
            {
                switch (item.KieuDuLieu)
                {
                    case "dropdown":
                        InsertFieldPostList.Add(@$"_FormData.append('{item.TenTruong}Id', $(""#{item.TenTruong}Id"").val())");
                        break;
                    case "date":
                    case "datetime":
                    case "bolean":
                    case "Double":
                    case "Int":
                    case "Float":
                        InsertFieldPostList.Add(@$"_FormData.append('{item.TenTruong}', $(""#{item.TenTruong}"").val())");
                        break;
                    case "image":
                        InsertFieldPostList.Add(@$"_FormData.append('{item.TenTruong}', $('input[name=""{item.TenTruong}""]')[0].files[0])");
                        break;
                    case "table":
                        item.TenBang = item.TenBang.Trim();
                        InsertFieldPostList.Add(@$"
                                App{_CreateObject.Name}.$data.Items{item.TenBang}.forEach((item, index) => {{
                                    Object.keys(item).forEach(key => {{
                                        _FormData.append(`Items{item.TenBang}[${{index}}][${{key}}]`, item[key])
                                      console.log(key, item[key]);
                                    }});
                            
                                }});

                            ");
                        InsertFieldPostList.Add(@$"
                            if(App{_CreateObject.Name}.$data.ListItems{item.TenBang}NeedDelete.length>0)
                            {{
                                _FormData.append('ListItems{item.TenBang}NeedDelete', App{_CreateObject.Name}.$data.ListItems{item.TenBang}NeedDelete);
                            }}
                        ");
                        //TODO create to type table

                        break;
                    default:
                        InsertFieldPostList.Add(@$"_FormData.append('{item.TenTruong}', $(""#{item.TenTruong}"").val())");
                        break;
                }
            }
            if (InsertFieldPostList.Count() > 0)
            {
                InsertFieldPostListString = String.Join(";\n", InsertFieldPostList);
            }
            new_file_js_CRUD_content = new_file_js_CRUD_content.Replace("//[INSERT_FIELD_SEND_POST]", InsertFieldPostListString);

            System.IO.File.WriteAllText(new_file_js_CRUD, new_file_js_CRUD_content);
            string new_file_js_Datatable = Path.Combine(ProjectPath, "wwwroot\\js\\" + Name + "\\" + Name + "_Datatable.js");
            System.IO.File.Copy(
                Path.Combine(ProjectPath, "wwwroot\\js\\" + ViewSource + "\\" + ViewSource + "_Datatable.js"),
                new_file_js_Datatable,
                true);
            string new_file_js_Datatable_content = System.IO.File.ReadAllText(new_file_js_Datatable);
            new_file_js_Datatable_content = new_file_js_Datatable_content.Replace(ViewSource, Name);
            //insert new fieds
            List<string> list_new_field = new List<string>();
            foreach (var item in ObjectFields)
            {
                if (item.HienThiTrongBang)
                {
                    string TenTruong = item.TenTruong;
                    if (TenTruong.Any(Char.IsWhiteSpace))
                    {
                        TenTruong = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.TenTruong.ToLower());
                        TenTruong = Utility.RemoveUnicode(TenTruong);
                        TenTruong = String.Concat(TenTruong.Where(c => !Char.IsWhiteSpace(c)));
                    }
                    switch (item.KieuDuLieu)
                    {
                        case "dropdown":
                            list_new_field.Add($"{{ \"data\": \"{TenTruong}Display\", \"name\": \"{TenTruong}Display\" }}");
                            break;
                        case "image":
                            list_new_field.Add(@$"
                                {{
                                    data: ""{TenTruong}Path"", ""name"": ""{TenTruong}Path"", render: function (data, type, row) {{
                                        return ""<img style='width:100px;height:100px' src='/"" + row.{TenTruong}Path +""'/>"";
                                    }}
                                }}
                            ");
                            break;
                        case "table":
                            //do not anything
                            break;
                        default:
                            list_new_field.Add($"{{ \"data\": \"{TenTruong}\", \"name\": \"{TenTruong}\" }}");
                            break;
                    }


                }

            }
            string new_fields = String.Join(",\n", list_new_field);
            new_file_js_Datatable_content = new_file_js_Datatable_content.Replace("//[INSERT_FIELD_SHOW_TABLE]", new_fields + ",");

            System.IO.File.WriteAllText(new_file_js_Datatable, new_file_js_Datatable_content);
        }
        public void WriteLesscript(string ProjectPath, CreateObject _CreateObject, string Name, List<ObjectFieldCRUDViewModel> ObjectFields, string ViewSource = "AssetCategories")
        {
            string css_path = Path.Combine(ProjectPath, $"wwwroot\\css\\views\\", Name);
            Directory.CreateDirectory(css_path);

            string new_file_css_detai = Path.Combine(ProjectPath, $"wwwroot\\css\\views\\{Name}\\Detail{Name}.less");
            System.IO.File.Copy(
                Path.Combine(ProjectPath, $"wwwroot\\css\\views\\{ViewSource}\\Detail{ViewSource}.less"),
                new_file_css_detai,
                true);
            string new_file_css_detai_content = System.IO.File.ReadAllText(new_file_css_detai);
            new_file_css_detai_content = new_file_css_detai_content.Replace(ViewSource, Name);
            System.IO.File.WriteAllText(new_file_css_detai, new_file_css_detai_content);

            string new_file_css_edit = Path.Combine(ProjectPath, $"wwwroot\\css\\views\\{Name}\\Edit{Name}.less");
            System.IO.File.Copy(
                Path.Combine(ProjectPath, $"wwwroot\\css\\views\\{ViewSource}\\Edit{ViewSource}.less"),
                new_file_css_edit,
                true);
            string new_file_css_edit_content = System.IO.File.ReadAllText(new_file_css_edit);
            new_file_css_edit_content = new_file_css_detai_content.Replace(new_file_css_edit_content, Name);
            System.IO.File.WriteAllText(new_file_css_edit, new_file_css_edit_content);

            string new_file_css_index = Path.Combine(ProjectPath, $"wwwroot\\css\\views\\{Name}\\Index{Name}.less");
            System.IO.File.Copy(
                Path.Combine(ProjectPath, $"wwwroot\\css\\views\\{ViewSource}\\Index{ViewSource}.less"),
                new_file_css_index,
                true);

            string new_file_css_index_content = System.IO.File.ReadAllText(new_file_css_index);
            new_file_css_index_content = new_file_css_detai_content.Replace(new_file_css_index_content, Name);
            System.IO.File.WriteAllText(new_file_css_index, new_file_css_index_content);

        }
        public void WriteController(string ProjectPath, CreateObject _CreateObject, string Name, List<ObjectFieldCRUDViewModel> ObjectFields, string ViewSource = "AssetCategories")
        {

            string new_controller_path = Path.Combine(ProjectPath, "Controllers\\" + Name + "Controller.cs");
            System.IO.File.Copy(
               Path.Combine(ProjectPath, "Controllers\\" + ViewSource + "Controller.cs"),
               new_controller_path,
               true);
            string new_controller_content = System.IO.File.ReadAllText(new_controller_path);
            new_controller_content = new_controller_content.Replace(ViewSource, Name);
            //insert 123
            var DropdownContent = "";
            var InsertUsing = "";
            List<string> InsertUsingList = new List<string>();
            string InsertLoad = "";
            string InsertLoadTreeView = "";
            string SaveImageField = "";
            string InsertImageField = "";
            string SaveFieldSubItems = "";
            string SaveInsertFieldSubItems = "";
            var ParentAndChildrent = ObjectFields.Where(item => item.TenTruong.Equals("ParentId")).FirstOrDefault();
            if (ParentAndChildrent != null)
            {
                var MainFieldParent = ParentAndChildrent.ListFieds.Where(field => field.Selected == true).First();
                var contentParentAndChildren = $@"
                    public IQueryable<{Name}CRUDViewModel> Load{Name}ChildrentByParentId(int ParentId, int ExcludeId = 0)
                    {{
                        return (from _{Name} in _context.{Name}.Where(x => x.ParentId == ParentId && x.Id != ExcludeId)
                            .OrderBy(x => x.Id)
                                select new {Name}CRUDViewModel
                                {{
                                    Id = _{Name}.Id,
                                    ParentId = _{Name}.ParentId,
                                    {MainFieldParent.TenTruong} = _{Name}.{MainFieldParent.TenTruong}
                                }});
                    }}
                    private List<{Name}CRUDViewModel> GetGridItem()
                    {{
                        List<{Name}CRUDViewModel> {Name}CRUDViewModelList = new List<{Name}CRUDViewModel>();
                        Load{Name}Childrent(ref {Name}CRUDViewModelList, 0, 1);

                        return {Name}CRUDViewModelList;
                    }}
                    public void Load{Name}Childrent(ref List<{Name}CRUDViewModel> {Name}CRUDViewModelList, int level = 0, int ParentId = 1, int ExcludeId = 0)
                    {{
                        try
                        {{
                            IQueryable<{Name}CRUDViewModel> _{Name}CRUDViewModelList = Load{Name}ChildrentByParentId(ParentId, ExcludeId);
                            var level1 = level + 1;
                            if (_{Name}CRUDViewModelList.Count() > 0)
                            {{
                                foreach (var item in _{Name}CRUDViewModelList)
                                {{
                                    item.{MainFieldParent.TenTruong} = ""---"".Repeat((uint)level) + item.{MainFieldParent.TenTruong};
                                    {Name}CRUDViewModelList.Add(item);
                                    Load{Name}Childrent(ref {Name}CRUDViewModelList, level1, item.Id, ExcludeId);
                                }}
                            }}

                        }}
                        catch (Exception ex)
                        {{
                            throw ex;
                        }}

                    }}

                ";
                var startIndex = new_controller_content.IndexOf("GetGridItemStart") + "GetGridItemStart".Length;
                var endIndex = new_controller_content.IndexOf("GetGridItemEnd") - 2;
                new_controller_content = new_controller_content.Substring(0, startIndex) + contentParentAndChildren + new_controller_content.Substring(endIndex);


                startIndex = new_controller_content.IndexOf("NOT_IN_TREE_VIEW_START") + "NOT_IN_TREE_VIEW_START".Length;
                endIndex = new_controller_content.IndexOf("NOT_IN_TREE_VIEW_END") - 2;
                new_controller_content = new_controller_content.Substring(0, startIndex) + new_controller_content.Substring(endIndex);
            }
            foreach (var ObjectField in ObjectFields)
            {

                switch (ObjectField.KieuDuLieu)
                {
                    case "dropdown":
                        var MainField = ObjectField.ListFieds.Where(field => field.Selected == true).First();
                        var TreeView = ObjectField.ListFieds.Where(field => field.TenTruong == "ParentId").Count() >= 1 ? true : false;
                        if (TreeView)
                        {

                            DropdownContent += @$"
                                    List<{ObjectField.TenTruong}CRUDViewModel> {ObjectField.TenTruong}CRUDViewModelList = new List<{ObjectField.TenTruong}CRUDViewModel>();
                                    {ObjectField.TenTruong}CRUDViewModelList.Add(new {ObjectField.TenTruong}CRUDViewModel
                                    {{
                                        Id = 1,
                                        ParentId = 0,
                                        {MainField.TenTruong} = ""Root"",
                                    }});
                                    if (id > 0)
                                    {{
                                        Load{ObjectField.TenTruong}(ref {ObjectField.TenTruong}CRUDViewModelList, 1, 1, id);
                                    }}
                                    else
                                    {{
                                        Load{ObjectField.TenTruong}(ref {ObjectField.TenTruong}CRUDViewModelList, 1, 1);
                                    }}
                                    ViewBag._Load{ObjectField.TenTruong} = new SelectList({ObjectField.TenTruong}CRUDViewModelList, ""Id"", ""{MainField.TenTruong}"");
                                    ";
                            InsertLoad += @$"
                                    public IQueryable<{ObjectField.TenTruong}CRUDViewModel> Load{ObjectField.TenTruong}ByParentId(int ParentId, int ExcludeId = 0)
                                    {{
                                        return (from _{ObjectField.TenTruong} in _context.{ObjectField.TenTruong}.Where(x => x.ParentId == ParentId && x.Id != ExcludeId)
                                            .OrderBy(x => x.Id)
                                            select new {ObjectField.TenTruong}CRUDViewModel
                                            {{
                                                Id = _{ObjectField.TenTruong}.Id,
                                                ParentId = _{ObjectField.TenTruong}.ParentId,
                                                {MainField.TenTruong} = _{ObjectField.TenTruong}.{MainField.TenTruong}
                                            }});
                                    }}
                                    public void Load{ObjectField.TenTruong}(ref List<{ObjectField.TenTruong}CRUDViewModel> {ObjectField.TenTruong}CRUDViewModelList, int level = 0, int ParentId = 1, int ExcludeId = 0)
                                    {{
                                        try{{
                                            IQueryable<{ObjectField.TenTruong}CRUDViewModel> _{ObjectField.TenTruong}CRUDViewModelList = Load{ObjectField.TenTruong}ByParentId(ParentId, ExcludeId);
                                            var level1 = level + 1;
                                            if (_{ObjectField.TenTruong}CRUDViewModelList.Count() > 0)
                                            {{
                                                    foreach (var item in _{ObjectField.TenTruong}CRUDViewModelList)
                                                    {{
                                                            
                                                            item.{MainField.TenTruong} = ""---"".Repeat((uint)level) + item.{MainField.TenTruong};
                                                            {ObjectField.TenTruong}CRUDViewModelList.Add(item);
                                                            Load{ObjectField.TenTruong}(ref {ObjectField.TenTruong}CRUDViewModelList, level1, item.Id, ExcludeId);
                                                    }}
                                            }}
                                        }}catch (Exception ex)
                                        {{
                                            throw ex;
                                        }}
                                    }}";
                        }
                        else
                        {
                            DropdownContent += $"\r\nViewBag._Load{ObjectField.TenTruong} = new SelectList(Load{ObjectField.TenTruong}(), \"Id\", \"Name\");\r\n";
                            InsertLoad += @$"
                                    public IQueryable<ItemDropdownListViewModel> Load{ObjectField.TenTruong}()
                                    {{
                                        return (from tblObj in _context.{ObjectField.TenTruong}.Where(x => x.Cancelled == false).OrderBy(x => x.Id)    
                                            select new ItemDropdownListViewModel    
                                            {{
                                                Id = tblObj.Id,
                                                Name = tblObj.{MainField.TenTruong}
                                            }}
                                        );
                                }}";

                        }
                        if (InsertUsingList.FirstOrDefault(a_item => a_item.Contains($"{ObjectField.TenTruong}ViewModel")) == null)
                        {
                            InsertUsingList.Add($"using AMS.Models.{ObjectField.TenTruong}ViewModel;\r\n");
                        }

                        break;
                    case "table":
                        ObjectField.TenBang = ObjectField.TenBang.Trim();

                        string InsertSubViewField = "";
                        var InsertSubViewFieldList = new List<string>();
                        var sub_items = LoadSubViewCRUDViewModelByOjbectFieldId(ObjectField.Id).ToList();
                        foreach (var sub_item in sub_items)
                        {
                            switch (sub_item.KieuDuLieu)
                            {
                                case "dropdown":


                                    InsertSubViewFieldList.Add(@$"{sub_item.TenTruong}Id = tblObj{ObjectField.TenBang}.{sub_item.TenTruong}Id");
                                    var ListFieds = _iCommon.LoadFieldsByTableName(sub_item.TenTruong);
                                    var IsSubViewTableParent = ListFieds.Where(item => item.Equals("ParentId")).FirstOrDefault();
                                    var MainSubViewFieldParent = sub_item.ListDisplaySubViewFieldOfTable.FirstOrDefault();
                                    var contentParentAndChildren = "";
                                    var DropdownSubViewContent = "";
                                    if (IsSubViewTableParent != null)
                                    {
                                        DropdownSubViewContent = @$"
                                            List<{sub_item.TenTruong}CRUDViewModel> {sub_item.TenTruong}SubViewCRUDViewModelList = new List<{sub_item.TenTruong}CRUDViewModel>();
                                            {sub_item.TenTruong}SubViewCRUDViewModelList.Add(new {sub_item.TenTruong}CRUDViewModel
                                            {{
                                                Id = 1,
                                                ParentId = 0,
                                                {MainSubViewFieldParent.TenTruong} = ""Root"",
                                            }});
                                            if (id > 0)
                                            {{
                                                Load{sub_item.TenTruong}SubView(ref {sub_item.TenTruong}SubViewCRUDViewModelList, 1, 1, id);
                                            }}
                                            else
                                            {{
                                                Load{sub_item.TenTruong}SubView(ref {sub_item.TenTruong}SubViewCRUDViewModelList, 1, 1);
                                            }}
                                            ViewBag.JsonItems{sub_item.TenTruong}SubView = JsonConvert.SerializeObject({sub_item.TenTruong}SubViewCRUDViewModelList);
                                            
                                        ";

                                        contentParentAndChildren = $@"
                                        public IQueryable<{sub_item.TenTruong}CRUDViewModel> Load{sub_item.TenTruong}SubViewByParentId(int ParentId, int ExcludeId = 0)
                                        {{
                                            return (from _{sub_item.TenTruong} in _context.{sub_item.TenTruong}.Where(x => x.ParentId == ParentId && x.Id != ExcludeId)
                                                .OrderBy(x => x.Id)
                                                    select new {sub_item.TenTruong}CRUDViewModel
                                                    {{
                                                        Id = _{sub_item.TenTruong}.Id,
                                                        ParentId = _{sub_item.TenTruong}.ParentId,
                                                        {MainSubViewFieldParent.TenTruong} = _{sub_item.TenTruong}.{MainSubViewFieldParent.TenTruong}
                                                    }});
                                        }}
                                         public void Load{sub_item.TenTruong}SubView(ref List<{sub_item.TenTruong}CRUDViewModel> {sub_item.TenTruong}CRUDViewModelList, int level = 0, int ParentId = 1, int ExcludeId = 0)
                                        {{
                                            try
                                            {{
                                                IQueryable<{sub_item.TenTruong}CRUDViewModel> _{sub_item.TenTruong}CRUDViewModelList = Load{sub_item.TenTruong}SubViewByParentId(ParentId, ExcludeId);
                                                var level1 = level + 1;
                                                if (_{sub_item.TenTruong}CRUDViewModelList.Count() > 0)
                                                {{
                                                    foreach (var item in _{sub_item.TenTruong}CRUDViewModelList)
                                                    {{
                                                        item.{MainSubViewFieldParent.TenTruong} = ""---"".Repeat((uint)level) + item.{MainSubViewFieldParent.TenTruong};
                                                        {sub_item.TenTruong}CRUDViewModelList.Add(item);
                                                        Load{sub_item.TenTruong}SubView(ref {sub_item.TenTruong}CRUDViewModelList, level1, item.Id, ExcludeId);
                                                    }}
                                                }}

                                            }}
                                            catch (Exception ex)
                                            {{
                                                throw ex;
                                            }}

                                        }}
                                        ";
                                        if (InsertUsingList.FirstOrDefault(a_item => a_item.Contains($"{sub_item.TenTruong}ViewModel")) == null)
                                        {
                                            InsertUsingList.Add($"using AMS.Models.{sub_item.TenTruong}ViewModel;\r\n");
                                        }
                                    }
                                    else
                                    {
                                        DropdownSubViewContent = @$"
                                            ViewBag.JsonItems{sub_item.TenTruong}SubView = JsonConvert.SerializeObject(Load{sub_item.TenTruong}SubView());
                                        ";
                                        contentParentAndChildren = $@"
                                            public IQueryable<{sub_item.TenTruong}CRUDViewModel> Load{sub_item.TenTruong}SubView()
                                            {{
                                                return (from tblObj{sub_item.TenTruong} in _context.{sub_item.TenTruong}.OrderBy(x => x.Id)
                                                        select new {sub_item.TenTruong}CRUDViewModel
                                                        {{
                                                            Id = tblObj{sub_item.TenTruong}.Id,
                                                            {MainSubViewFieldParent.TenTruong} = tblObj{sub_item.TenTruong}.{MainSubViewFieldParent.TenTruong}
                                                           
                                                        }}
                                                );
                                            }}
                                        ";
                                        if (InsertUsingList.FirstOrDefault(a_item => a_item.Contains($"{sub_item.TenTruong}ViewModel")) == null)
                                        {
                                            InsertUsingList.Add($"using AMS.Models.{sub_item.TenTruong}ViewModel;\r\n");
                                        }
                                    }



                                    new_controller_content = new_controller_content.Replace("//[INSERT_LOAD]", $"//[INSERT_LOAD]\r\n{contentParentAndChildren}");
                                    new_controller_content = new_controller_content.Replace("//[INSERT_OBJECTSUBVIEWFIELDLIST]", $"//[INSERT_OBJECTSUBVIEWFIELDLIST]\r\n{DropdownSubViewContent}");

                                    break;
                                case "image":
                                    InsertSubViewFieldList.Add(@$"{sub_item.TenTruong}Path = tblObj{ObjectField.TenBang}.{sub_item.TenTruong}Path");
                                    break;
                                case "date":
                                case "datetime":
                                case "bolean":
                                case "Double":
                                case "Int":
                                case "Float":
                                case "string":
                                    InsertSubViewFieldList.Add(@$"{sub_item.TenTruong} = tblObj{ObjectField.TenBang}.{sub_item.TenTruong}");
                                    break;
                            }
                        }
                        if (InsertSubViewFieldList.Count() > 0)
                        {
                            InsertSubViewField = String.Join(",\n", InsertSubViewFieldList) + ",";
                        }

                        DropdownContent += $"\r\nViewBag.JsonItems{ObjectField.TenBang} = JsonConvert.SerializeObject(Load{ObjectField.TenBang}(vm.Id).ToList());\r\n";
                        InsertLoad += @$"
                                    public IQueryable<{ObjectField.TenBang}CRUDViewModel> Load{ObjectField.TenBang}(int Id)
                                    {{
                                        return (from tblObj{ObjectField.TenBang} in _context.{ObjectField.TenBang}.Where(x => x.{Name}Id == Id).OrderBy(x => x.Id)    
                                            select new {ObjectField.TenBang}CRUDViewModel    
                                            {{
                                                Id = tblObj{ObjectField.TenBang}.Id,
                                                {Name}Id = tblObj{ObjectField.TenBang}.{Name}Id,
                                                {InsertSubViewField}
                                            }}
                                        );
                                }}";
                        if (InsertUsingList.FirstOrDefault(a_item => a_item.Contains($"{ObjectField.TenBang}ViewModel")) == null)
                        {
                            InsertUsingList.Add($"using AMS.Models.{ObjectField.TenBang}ViewModel;\r\n");
                        }



                        SaveFieldSubItems += $@"
                                List<{ObjectField.TenBang}CRUDViewModel> Items{ObjectField.TenBang} = vm.Items{ObjectField.TenBang};
                                foreach (var Item{ObjectField.TenBang} in Items{ObjectField.TenBang})
                                {{
                                    if(Item{ObjectField.TenBang}.Id>0){{
                                        var _Object{ObjectField.TenBang} = await _context.{ObjectField.TenBang}.FindAsync(Item{ObjectField.TenBang}.Id);
                                        _Object{ObjectField.TenBang}.ModifiedBy = _UserName;
                                        _context.Entry(_Object{ObjectField.TenBang}).CurrentValues.SetValues(Item{ObjectField.TenBang});
                                        await _context.SaveChangesAsync();

                                    }}else{{
                                        {ObjectField.TenBang} _{ObjectField.TenBang}Object = Item{ObjectField.TenBang};
                                        _{ObjectField.TenBang}Object.{Name}Id = _{Name}.Id;
                                        _{ObjectField.TenBang}Object.CreatedDate = DateTime.Now;
                                        _{ObjectField.TenBang}Object.CreatedBy = _UserName;
                                        _{ObjectField.TenBang}Object.ModifiedDate = DateTime.Now;
                                        _{ObjectField.TenBang}Object.ModifiedBy = _UserName;
                                         _context.Add(_{ObjectField.TenBang}Object);
                                        await _context.SaveChangesAsync();
                                    }}

                                }}
                                if (vm.ListItems{ObjectField.TenBang}NeedDelete != null) foreach (var Id in vm.ListItems{ObjectField.TenBang}NeedDelete)
                                {{
                                    {ObjectField.TenBang} _{ObjectField.TenBang}Object = await _context.{ObjectField.TenBang}.FindAsync(Id);
                                    _context.{ObjectField.TenBang}.Remove(_{ObjectField.TenBang}Object);
                                    await _context.SaveChangesAsync();
                                }}

                        ";

                        SaveInsertFieldSubItems += $@"
                                List<{ObjectField.TenBang}CRUDViewModel> Items{ObjectField.TenBang} = vm.Items{ObjectField.TenBang};
                                foreach (var Item{ObjectField.TenBang} in Items{ObjectField.TenBang})
                                {{
                                    {ObjectField.TenBang} _{ObjectField.TenBang}Object = Item{ObjectField.TenBang};
                                    _{ObjectField.TenBang}Object.{Name}Id = _{Name}.Id;
                                    _{ObjectField.TenBang}Object.CreatedDate = DateTime.Now;
                                    _{ObjectField.TenBang}Object.CreatedBy = _UserName;
                                    _{ObjectField.TenBang}Object.ModifiedDate = DateTime.Now;
                                    _{ObjectField.TenBang}Object.ModifiedBy = _UserName;
                                     _context.Add(_{ObjectField.TenBang}Object);
                                    await _context.SaveChangesAsync();
                                }}
                        ";

                        break;
                    case "image":
                        SaveImageField += @$"
                        var {ObjectField.TenTruong}Path = await _iCommon.UploadedFile(vm.{ObjectField.TenTruong});
                        if ({ObjectField.TenTruong}Path != null)
                        {{
                            vm.{ObjectField.TenTruong}Path = ""/upload/"" + {ObjectField.TenTruong}Path;
                        }}
                        else
                        {{
                            vm.{ObjectField.TenTruong}Path = _{Name}.{ObjectField.TenTruong}Path;
                        }}
                        ";
                        InsertImageField += @$"
                            var {ObjectField.TenTruong}Path = await _iCommon.UploadedFile(vm.{ObjectField.TenTruong});
                                if ({ObjectField.TenTruong}Path != null)
                                {{
                                    _{Name}.{ObjectField.TenTruong}Path = ""/upload/"" + {ObjectField.TenTruong}Path;
                                }}
                                else
                                {{
                                    {ObjectField.TenTruong}Path = {ObjectField.TenTruong}Path != null ? {ObjectField.TenTruong}Path : ""blank-asset.png"";
                                    _{Name}.{ObjectField.TenTruong}Path = ""/upload/"" + {ObjectField.TenTruong}Path;
                                }}
                        ";
                        break;
                    case "ParentId":
                        //TODO: fix here
                        var MainFieldParent = ObjectField.ListFieds.Where(field => field.Selected == true).First();
                        DropdownContent += @$"
                            List<{Name}CRUDViewModel> {Name}CRUDViewModelList = new List<{Name}CRUDViewModel>();
                            {Name}CRUDViewModelList.Add(new {Name}CRUDViewModel
                            {{
                                Id = 1,
                                ParentId = 0,
                                {MainFieldParent.TenTruong} = ""Root"",
                            }});
                            if (id > 0)
                            {{
                                Load{Name}(ref {Name}CRUDViewModelList, 1, 1, id);
                            }}
                            else
                            {{
                                Load{Name}(ref {Name}CRUDViewModelList, 1, 1);
                            }}
                            ViewBag._Load{Name} = new SelectList({Name}CRUDViewModelList, ""Id"", ""{MainFieldParent.TenTruong}"");
                            ";
                        InsertLoad += @$"
                            public IQueryable<{Name}CRUDViewModel> Load{Name}ByParentId(int ParentId, int ExcludeId = 0)
                            {{
                                return (from _{Name} in _context.{Name}.Where(x => x.ParentId == ParentId && x.Id != ExcludeId)
                                    .OrderBy(x => x.Id)
                                    select new {Name}CRUDViewModel
                                    {{
                                        Id = _{Name}.Id,
                                        ParentId = _{Name}.ParentId,
                                        {MainFieldParent.TenTruong} = _{Name}.{MainFieldParent.TenTruong}
                                    }});
                            }}
                            public void Load{Name}(ref List<{Name}CRUDViewModel> {Name}CRUDViewModelList, int level = 0, int ParentId = 1, int ExcludeId = 0)
                            {{
                                try{{
                                    IQueryable<{Name}CRUDViewModel> _{Name}CRUDViewModelList = Load{Name}ByParentId(ParentId, ExcludeId);
                                    var level1 = level + 1;
                                    if (_{Name}CRUDViewModelList.Count() > 0)
                                    {{
                                            foreach (var item in _{Name}CRUDViewModelList)
                                            {{
                                                    item.{MainFieldParent.TenTruong} = ""---"".Repeat((uint)level) + item.{MainFieldParent.TenTruong};
                                                    {Name}CRUDViewModelList.Add(item);
                                                    Load{Name}(ref {Name}CRUDViewModelList, level1, item.Id, ExcludeId);
                                            }}
                                    }}
                                }}catch (Exception ex)
                                {{
                                    throw ex;
                                }}
                            }}";


                        break;
                    default:

                        break;
                }

            }
            if (InsertUsingList.Count() > 0)
            {
                InsertUsing = String.Join("\r\n", InsertUsingList);
            }
            new_controller_content = new_controller_content.Replace("//[INSERT_SAVE_FIELD_IMAGE_CONTROLLER]", SaveImageField);
            new_controller_content = new_controller_content.Replace("//[INSERT_INSERT_FIELD_IMAGE_CONTROLLER]", InsertImageField);
            new_controller_content = new_controller_content.Replace("//[INSERT_SAVE_FIELD_SUB_ITEM_VIEW_CONTROLLER]", SaveFieldSubItems);
            new_controller_content = new_controller_content.Replace("//[INSERT_SAVE_INSERT_FIELD_SUB_ITEM_VIEW_CONTROLLER]", SaveInsertFieldSubItems);
            new_controller_content = new_controller_content.Replace("//[INSERT_LOAD]", $"//[INSERT_LOAD]\r\n{InsertLoad}");
            new_controller_content = new_controller_content.Replace("//[INSERT_OBJECTFIELDLIST]", DropdownContent);
            new_controller_content = new_controller_content.Replace("//[INSERT_USING]", InsertUsing);

            //insert new fieds of controller 1
            List<string> list_new_field_of_controller_view_1 = new List<string>();
            foreach (var item in ObjectFields)
            {
                string TenTruong = item.TenTruong;
                string KieuDuLieu = "";
                if (item.KieuDuLieu != "dropdown")
                {
                    if (TenTruong.Any(Char.IsWhiteSpace))
                    {
                        TenTruong = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.TenTruong.ToLower());
                        TenTruong = Utility.RemoveUnicode(TenTruong);
                        TenTruong = String.Concat(TenTruong.Where(c => !Char.IsWhiteSpace(c)));
                    }
                    KieuDuLieu = "";
                }
                switch (item.KieuDuLieu)
                {
                    case "dropdown":
                        break;
                    case "string":
                    case "textarea":
                        list_new_field_of_controller_view_1.Add($"|| obj.{TenTruong}.ToLower().Contains(searchValue)");
                        break;
                }

            }
            string new_fields_of_controller_view_1 = String.Join("\r\n", list_new_field_of_controller_view_1);
            new_controller_content = new_controller_content.Replace("//[INSERT_FIELD_CONTROLLER_1]", new_fields_of_controller_view_1);



            //insert new fieds of controller 2
            List<string> list_new_field_of_controller_view_2 = new List<string>();
            List<string> list_new_field_left_joint = new List<string>();
            foreach (var item in ObjectFields)
            {
                string TenTruong = item.TenTruong;
                string KieuDuLieu = "";
                if (item.KieuDuLieu != "dropdown")
                {
                    if (TenTruong.Any(Char.IsWhiteSpace))
                    {
                        TenTruong = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.TenTruong.ToLower());
                        TenTruong = Utility.RemoveUnicode(TenTruong);
                        TenTruong = String.Concat(TenTruong.Where(c => !Char.IsWhiteSpace(c)));
                    }
                }
                switch (item.KieuDuLieu)
                {
                    case "dropdown":
                        var TenTruongNoId = TenTruong;
                        var TenTruongId = $"{TenTruong}Id";
                        TenTruong = $"{TenTruong}Id";

                        list_new_field_left_joint.Add(@$"
                                join _{TenTruongNoId} in _context.{TenTruongNoId} on _{Name}.{TenTruongId} equals _{TenTruongNoId}.Id
                                into list{TenTruongNoId}
                                from _{TenTruongNoId} in list{TenTruongNoId}.DefaultIfEmpty()
                                ");
                        list_new_field_of_controller_view_2.Add($"{TenTruongNoId}Display = _{TenTruongNoId}.{item.ListFieds.Where(field => field.Selected).FirstOrDefault().TenTruong}");
                        break;
                    case "table":
                        //nothing to do
                        break;
                    case "image":
                        list_new_field_of_controller_view_2.Add($"{TenTruong}Path = _{Name}.{TenTruong}Path");
                        break;
                    default:
                        list_new_field_of_controller_view_2.Add($"{TenTruong} = _{Name}.{TenTruong}");
                        break;
                }


            }
            string new_fields_of_controller_view_2 = String.Join(",\r\n", list_new_field_of_controller_view_2);
            string str_new_field_left_joint = String.Join("\r\n", list_new_field_left_joint);
            new_controller_content = new_controller_content.Replace("//[INSERT_FIELD_CONTROLLER_2]", new_fields_of_controller_view_2 + ",");
            new_controller_content = new_controller_content.Replace("//[INSERT_FIELD_LEFT_JOINT_CONTROLLER]", str_new_field_left_joint);





            System.IO.File.WriteAllText(new_controller_path, new_controller_content);

        }
        public void WriteViewModel(string ProjectPath, CreateObject _CreateObject, string Name, List<ObjectFieldCRUDViewModel> ObjectFields, string ViewSource = "AssetCategories")
        {
            string ModelViewPathDirectory = Path.Combine(ProjectPath, "Models\\" + Name + "ViewModel");
            Directory.CreateDirectory(ModelViewPathDirectory);

            string new_model_view_path = Path.Combine(ModelViewPathDirectory, Name + "CRUDViewModel.cs");
            System.IO.File.Copy(
               Path.Combine(ProjectPath, "Models\\" + ViewSource + "ViewModel\\" + ViewSource + "CRUDViewModel.cs"),
               new_model_view_path,
               true);
            string new_model_view_content = System.IO.File.ReadAllText(new_model_view_path);
            new_model_view_content = new_model_view_content.Replace(ViewSource, Name);

            //insert new fieds of model view
            List<string> list_new_field_of_model_view = new List<string>();
            List<string> list_import_model_view = new List<string>();
            foreach (var item in ObjectFields)
            {
                item.Label = item.Label.Trim();
                string TenTruong = item.TenTruong;
                string KieuDuLieu = "";
                if (item.KieuDuLieu != "dropdown")
                {
                    if (TenTruong.Any(Char.IsWhiteSpace))
                    {
                        TenTruong = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.TenTruong.ToLower());
                        TenTruong = Utility.RemoveUnicode(TenTruong);
                        TenTruong = String.Concat(TenTruong.Where(c => !Char.IsWhiteSpace(c)));
                    }
                    KieuDuLieu = "";
                }
                string BatBuocNhap = item.BatBuocNhap ? "" : "?";
                switch (item.KieuDuLieu)
                {
                    case "dropdown":
                        KieuDuLieu = "int";
                        string TenTruongNoId = TenTruong;
                        string TenTruongId = $"{TenTruong}Id";
                        list_new_field_of_model_view.Add(@$"
                                    [Display(Name = ""{item.Label}"")]
                                    public string? {TenTruongNoId}Display {{ get; set; }}
                                    ");
                        list_new_field_of_model_view.Add(@$"
                                    [Display(Name = ""{item.Label}"")]
                                    public {KieuDuLieu}{BatBuocNhap} {TenTruongId} {{ get; set; }}
                                    ");
                        break;
                    case "date":
                    case "datetime":
                        KieuDuLieu = "DateTime";
                        list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{item.Label}"")] 
                                        public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}
                                    ");
                        break;
                    case "image":
                        list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{item.Label}"")] 
                                        public string? {TenTruong}Path {{ get; set; }}
                                    ");
                        list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{item.Label}"")] 
                                        public IFormFile{BatBuocNhap} {TenTruong} {{ get; set; }}
                                    ");
                        break;
                    case "bolean":
                        KieuDuLieu = "bool";
                        list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{item.Label}"")] 
                                        public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}
                                    ");
                        break;
                    case "table":
                        item.TenBang = item.TenBang.Trim();
                        list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{item.LabelTenBang}"")] 
                                        public List<{item.TenBang}CRUDViewModel>{BatBuocNhap} Items{item.TenBang} {{ get; set; }}
                                    ");
                        list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{item.LabelTenBang}"")] 
                                        public List<int>? ListItems{item.TenBang}NeedDelete {{ get; set; }}
                                    ");
                        list_import_model_view.Add($"using AMS.Models.{item.TenBang}ViewModel;");
                        break;
                    case "ParentId":
                        KieuDuLieu = "int";
                        list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{item.Label}"")] 
                                        public int ParentId {{ get; set; }}
                                    ");
                        break;
                    case "Double":
                        KieuDuLieu = "double";
                        list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{item.Label}"")] 
                                        public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}
                                    ");

                        break;

                    case "Float":
                        KieuDuLieu = "float";
                        list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{item.Label}"")] 
                                        public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}
                                    ");

                        break;
                    case "Int":
                        KieuDuLieu = "int";
                        list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{item.Label}"")] 
                                        public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}
                                    ");

                        break;
                    default:
                        KieuDuLieu = "string";
                        list_new_field_of_model_view.Add(@$"
                                    [Display (Name = ""{item.Label}"")] 
                                    public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}
                                ");
                        break;
                }


            }
            string new_fields_of_model_view = String.Join("\r\n", list_new_field_of_model_view);
            new_model_view_content = new_model_view_content.Replace("//[INSERT_FIELD_MODEL_VIEW]", new_fields_of_model_view);
            string new_fields_of_model_view_str = "";
            if (list_import_model_view.Count() > 0)
            {
                new_fields_of_model_view_str = String.Join("\r\n", list_import_model_view);
            }
            new_model_view_content = new_model_view_content.Replace("//[INSERT_IMPORT_FIELD_MODEL_VIEW]", new_fields_of_model_view_str);




            //insert new fieds of model view implicit 1
            List<string> list_new_field_of_model_view_implicit_1 = new List<string>();
            foreach (var item in ObjectFields)
            {
                string TenTruong = item.TenTruong;
                string KieuDuLieu = "";
                if (item.KieuDuLieu != "dropdown")
                {
                    if (TenTruong.Any(Char.IsWhiteSpace))
                    {
                        TenTruong = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.TenTruong.ToLower());
                        TenTruong = Utility.RemoveUnicode(TenTruong);
                        TenTruong = String.Concat(TenTruong.Where(c => !Char.IsWhiteSpace(c)));
                    }
                }
                switch (item.KieuDuLieu)
                {
                    case "dropdown":
                        list_new_field_of_model_view_implicit_1.Add($"{TenTruong}Id = _{Name}.{TenTruong}Id");
                        break;
                    case "date":
                    case "datetime":
                        list_new_field_of_model_view_implicit_1.Add($"{TenTruong} = _{Name}.{TenTruong}");
                        break;
                    case "bolean":
                        list_new_field_of_model_view_implicit_1.Add($"{TenTruong} = _{Name}.{TenTruong}");
                        break;
                    case "table":
                        //nothing
                        break;
                    case "image":
                        list_new_field_of_model_view_implicit_1.Add($"{TenTruong}Path = _{Name}.{TenTruong}Path");
                        break;
                    default:
                        list_new_field_of_model_view_implicit_1.Add($"{TenTruong} = _{Name}.{TenTruong}");
                        break;
                }



            }
            string new_fields_of_model_view_implicit_1 = String.Join(",\r\n", list_new_field_of_model_view_implicit_1);
            new_model_view_content = new_model_view_content.Replace("//[INSERT_FIELD_MODEL_VIEW_IMPLICIT_1]", new_fields_of_model_view_implicit_1 + ",");


            //insert new fieds of model view implicit 2
            List<string> list_new_field_of_model_view_implicit_2 = new List<string>();
            foreach (var item in ObjectFields)
            {
                string TenTruong = item.TenTruong;
                string KieuDuLieu = "";
                if (item.KieuDuLieu != "dropdown")
                {
                    if (TenTruong.Any(Char.IsWhiteSpace))
                    {
                        TenTruong = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.TenTruong.ToLower());
                        TenTruong = Utility.RemoveUnicode(TenTruong);
                        TenTruong = String.Concat(TenTruong.Where(c => !Char.IsWhiteSpace(c)));
                    }
                }
                switch (item.KieuDuLieu)
                {
                    case "dropdown":
                        list_new_field_of_model_view_implicit_2.Add($"{TenTruong}Id = vm.{TenTruong}Id");
                        break;
                    case "table":
                        //nothing
                        break;
                    case "image":
                        list_new_field_of_model_view_implicit_2.Add($"{TenTruong}Path = vm.{TenTruong}Path");
                        break;
                    default:
                        list_new_field_of_model_view_implicit_2.Add($"{TenTruong} = vm.{TenTruong}");
                        break;
                }


            }
            string new_fields_of_model_view_implicit_2 = String.Join(",\r\n", list_new_field_of_model_view_implicit_2);
            new_model_view_content = new_model_view_content.Replace("//[INSERT_FIELD_MODEL_VIEW_IMPLICIT_2]", new_fields_of_model_view_implicit_2 + ",");




            System.IO.File.WriteAllText(new_model_view_path, new_model_view_content);
        }
        public void WriteSubViewModel(string ProjectPath, CreateObject _CreateObject, string Name, List<ObjectFieldCRUDViewModel> ObjectFields, string ViewSource = "AssetCategories")
        {



            foreach (var item in ObjectFields)
            {
                if (item.KieuDuLieu == "table")
                {
                    item.TenBang = item.TenBang.Trim();
                    item.Label = item.Label.Trim();
                    //insert new fieds of model view
                    List<string> list_new_field_of_model_view = new List<string>();
                    string ModelViewPathDirectory = Path.Combine(ProjectPath, "Models\\" + item.TenBang + "ViewModel");
                    Directory.CreateDirectory(ModelViewPathDirectory);

                    string new_model_view_path = Path.Combine(ModelViewPathDirectory, item.TenBang + "CRUDViewModel.cs");
                    System.IO.File.Copy(
                       Path.Combine(ProjectPath, "Models\\" + ViewSource + "ViewModel\\" + ViewSource + "CRUDViewModel.cs"),
                       new_model_view_path,
                       true);
                    string new_model_view_content = System.IO.File.ReadAllText(new_model_view_path);
                    new_model_view_content = new_model_view_content.Replace(ViewSource, item.TenBang);
                    var sub_items = item.sub_items;
                    List<string> list_new_field_of_model_view_implicit_1 = new List<string>();
                    List<string> list_new_field_of_model_view_implicit_2 = new List<string>();
                    list_new_field_of_model_view.Add(@$"
                                    [Display(Name = ""{item.Label}"")]
                                    public int {Name}Id {{ get; set; }}
                                    ");
                    list_new_field_of_model_view_implicit_1.Add($"{Name}Id = _{item.TenBang}.{Name}Id");
                    list_new_field_of_model_view_implicit_2.Add($"{Name}Id = vm.{Name}Id");
                    foreach (var sub_item in sub_items)
                    {
                        string TenTruong = sub_item.TenTruong;
                        string KieuDuLieu = "";
                        string BatBuocNhap = sub_item.BatBuocNhap ? "" : "?";
                        switch (sub_item.KieuDuLieu)
                        {
                            case "dropdown":
                                KieuDuLieu = "int";
                                list_new_field_of_model_view.Add(@$"
                                    [Display(Name = ""{sub_item.Label}"")]
                                    public string? {TenTruong}Display {{ get; set; }}
                                    ");
                                list_new_field_of_model_view.Add(@$"
                                    [Display(Name = ""{sub_item.Label}"")]
                                    public {KieuDuLieu}{BatBuocNhap} {TenTruong}Id {{ get; set; }}
                                    ");
                                break;
                            case "date":
                            case "datetime":
                                KieuDuLieu = "DateTime";
                                list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{sub_item.Label}"")] 
                                        public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}
                                    ");
                                break;
                            case "bolean":
                                KieuDuLieu = "bool";
                                list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{sub_item.Label}"")] 
                                        public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}
                                    ");
                                break;
                            case "ParentId":
                                KieuDuLieu = "int";
                                list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{sub_item.Label}"")] 
                                        public int ParentId {{ get; set; }}
                                    ");
                                break;
                            case "Double":
                                KieuDuLieu = "double";
                                list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{sub_item.Label}"")] 
                                        public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}
                                    ");

                                break;
                            case "Float":
                                KieuDuLieu = "float";
                                list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{sub_item.Label}"")] 
                                        public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}
                                    ");

                                break;
                            case "Int":
                                KieuDuLieu = "int";
                                list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{sub_item.Label}"")] 
                                        public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}
                                    ");

                                break;
                            case "image":
                                list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{sub_item.Label}"")] 
                                        public string? {TenTruong}Path {{ get; set; }}
                                    ");
                                list_new_field_of_model_view.Add(@$"
                                        [Display (Name = ""{sub_item.Label}"")] 
                                        public IFormFile{BatBuocNhap} {TenTruong} {{ get; set; }}
                                    ");
                                break;
                            default:
                                KieuDuLieu = "string";
                                list_new_field_of_model_view.Add(@$"
                                    [Display (Name = ""{sub_item.Label}"")] 
                                    public {KieuDuLieu}{BatBuocNhap} {TenTruong} {{ get; set; }}
                                ");
                                break;
                        }




                        switch (sub_item.KieuDuLieu)
                        {
                            case "dropdown":
                                list_new_field_of_model_view_implicit_1.Add($"{TenTruong}Id = _{item.TenBang}.{TenTruong}Id");
                                break;
                            case "date":
                            case "datetime":
                                list_new_field_of_model_view_implicit_1.Add($"{TenTruong} = _{item.TenBang}.{TenTruong}");
                                break;
                            case "bolean":
                                list_new_field_of_model_view_implicit_1.Add($"{TenTruong} = _{item.TenBang}.{TenTruong}");
                                break;
                            case "image":
                                list_new_field_of_model_view_implicit_1.Add($"{TenTruong}Path = _{item.TenBang}.{TenTruong}Path");
                                break;
                            default:
                                list_new_field_of_model_view_implicit_1.Add($"{TenTruong} = _{item.TenBang}.{TenTruong}");
                                break;
                        }
                        switch (sub_item.KieuDuLieu)
                        {
                            case "dropdown":
                                list_new_field_of_model_view_implicit_2.Add($"{TenTruong}Id = vm.{TenTruong}Id");
                                break;
                            case "date":
                            case "datetime":
                                list_new_field_of_model_view_implicit_2.Add($"{TenTruong} = vm.{TenTruong}");
                                break;
                            case "bolean":
                                list_new_field_of_model_view_implicit_2.Add($"{TenTruong} = vm.{TenTruong}");
                                break;
                            case "image":
                                list_new_field_of_model_view_implicit_2.Add($"{TenTruong}Path = vm.{TenTruong}Path");
                                break;
                            default:
                                list_new_field_of_model_view_implicit_2.Add($"{TenTruong} = vm.{TenTruong}");
                                break;
                        }

                    }
                    string new_fields_of_model_view_implicit_1 = String.Join(",\r\n", list_new_field_of_model_view_implicit_1);
                    new_model_view_content = new_model_view_content.Replace("//[INSERT_FIELD_MODEL_VIEW_IMPLICIT_1]", new_fields_of_model_view_implicit_1 + ",");

                    string new_fields_of_model_view = String.Join("\r\n", list_new_field_of_model_view);
                    new_model_view_content = new_model_view_content.Replace("//[INSERT_FIELD_MODEL_VIEW]", new_fields_of_model_view);
                    string new_fields_of_model_view_implicit_2 = String.Join(",\r\n", list_new_field_of_model_view_implicit_2);
                    new_model_view_content = new_model_view_content.Replace("//[INSERT_FIELD_MODEL_VIEW_IMPLICIT_2]", new_fields_of_model_view_implicit_2 + ",");




                    System.IO.File.WriteAllText(new_model_view_path, new_model_view_content);
                }



            }
        }
        public string RenderTable(ObjectFieldCRUDViewModel _ObjectFieldCRUDViewModel)
        {
            var ThsHtml = "";
            var TdsHtml = "";
            foreach (var SubView in _ObjectFieldCRUDViewModel.sub_items)
            {
                ThsHtml += @$"<th>{SubView.Label}</th>";
                switch (SubView.KieuDuLieu)
                {
                    case "dropdown":
                        SubView.ListDisplaySubViewFieldOfTable = _context.DisplaySubViewFieldOfTable.Where(ItemListDisplaySubViewFieldOfTable => ItemListDisplaySubViewFieldOfTable.ObjectSubViewFiledId == SubView.Id).ToList();
                        var ListFieds = _iCommon.LoadFieldsByTableName(SubView.TenTruong);
                        var MainSubViewFieldParent = SubView.ListDisplaySubViewFieldOfTable.FirstOrDefault();
                        TdsHtml += @$"
                            <td>
                                <select class=""form-control"" style=""width:100%;"" v-model=""item.{SubView.TenTruong}Id"">
                                    <option value=""0"">Select item</option>
                                    <option v-for=""(item{SubView.TenTruong},index{SubView.TenTruong}) in Items{SubView.TenTruong}SubView"" :value=""item{SubView.TenTruong}.Id"">{{{{item{SubView.TenTruong}.{MainSubViewFieldParent.TenTruong}}}}}</option>
                                </select>
                            </td>";
                        break;
                    case "image":
                        TdsHtml += @$"
                            <td>
                                <div class=""image-thumb"" @@click=""$refs.{SubView.TenTruong}.click()"">
                                    <img class=""image-thumb-upload"" src=""/images/DefaultAsset/upload.png"" />
                                    <input type=""hidden"" v-model=""item.{SubView.TenTruong}"" />
                                </div>
                            </td>
                            ";
                        break;
                    default:
                        TdsHtml += @$"
                           <td>
                                <input v-model=""item.{SubView.TenTruong}"" class=""form-control"" />
                            </td>";
                        break;
                }
            }
            var colspan = _ObjectFieldCRUDViewModel.sub_items.Count() + 1;
            var htmlRows = @$"
                <label>{_ObjectFieldCRUDViewModel.Label}</label>
                <table  class=""table table-{_ObjectFieldCRUDViewModel.TenTruong}"">
                    <thead>
                        <tr>
                            <th>STT</th>
                            {ThsHtml}
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for=""(item,index) in Items{_ObjectFieldCRUDViewModel.TenTruong}"">
                            <td>{{{{index+1}}}}</td>
                            {TdsHtml}
                            <td><button class=""btn btn-warning"" type=""button"" v-on:click=""RemoveRow{_ObjectFieldCRUDViewModel.TenTruong}($event,index)"">Xoá</button></td>
                        </tr>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan=""{colspan}"" class=""text-right""></td>
                            <td><button type=""button"" class=""btn btn-info"" v-on:click=""AddRow{_ObjectFieldCRUDViewModel.TenTruong}($event)"">Thêm</button></td>
                        </tr>
                    </tfoot>
                </table>";
            return htmlRows;
        }
        private static void ChangeContentFolder(string sourcePath, string Name, string ContentFile = "AssetCategories")
        {
            DirectoryInfo d = new DirectoryInfo(@sourcePath); //Assuming Test is your Folder

            FileInfo[] Files = d.GetFiles("*.cshtml"); //Getting Text files

            foreach (FileInfo file in Files)
            {
                string content = System.IO.File.ReadAllText(file.FullName);
                content = content.Replace(ContentFile, Name);
                System.IO.File.WriteAllText(file.FullName, content);
            }
        }
        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                System.IO.File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var _CreateObject = await _context.CreateObject.FindAsync(id);
                var ProjectPath = _iCommon.GetAppDevPath();
                await DeleteObjectView(ProjectPath, _CreateObject);
                await DeleteDatabaseObjectView(ProjectPath, _CreateObject);
                _context.CreateObject.Remove(_CreateObject);
                await _context.SaveChangesAsync();
                return new JsonResult(_CreateObject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task DeleteDatabaseObjectView(string ProjectPath, CreateObject _CreateObject)
        {
            try
            {
                var DatabaseName = _iCommon.GetDatabaseName();
                var TableName = _CreateObject.Name;
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = @$"
                                DROP TABLE IF EXISTS [{DatabaseName}].[dbo].[{TableName}]
                            ";
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();
                    command.ExecuteNonQuery();
                }
                var ListObjectField = _context.ObjectField.Where(item => item.CreateObjectId == _CreateObject.Id).ToList();
                foreach (var _ObjectField in ListObjectField)
                {
                    if (_ObjectField.KieuDuLieu == "table")
                    {
                        using (var command = _context.Database.GetDbConnection().CreateCommand())
                        {
                            command.CommandText = @$"
                                DROP TABLE IF EXISTS [{DatabaseName}].[dbo].[{_ObjectField.TenBang}]
                            ";
                            command.CommandType = CommandType.Text;
                            _context.Database.OpenConnection();
                            command.ExecuteNonQuery();
                        }
                    }
                    var ListSubViewField = _context.SubViewField.Where(item => item.OjbectFieldId == _ObjectField.Id).ToList();
                    foreach (var _SubViewField in ListSubViewField)
                    {
                        _context.SubViewField.Remove(_SubViewField);
                        await _context.SaveChangesAsync();
                    }
                    _context.ObjectField.Remove(_ObjectField);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task DeleteObjectView(string ProjectPath, CreateObject _CreateObject)
        {
            string js_path = Path.Combine(ProjectPath, "wwwroot\\js", _CreateObject.Name);
            if (Directory.Exists(js_path))
                Directory.Delete(js_path, true);

            string model_path = Path.Combine(ProjectPath, "Models\\" + _CreateObject.Name + ".cs");
            if (System.IO.File.Exists(js_path))
                System.IO.File.Delete(model_path);

            string ModelViewPathDirectory = Path.Combine(ProjectPath, "Models\\" + _CreateObject.Name + "ViewModel");
            string model_view_path = Path.Combine(ModelViewPathDirectory, _CreateObject.Name + "CRUDViewModel.cs");
            if (System.IO.File.Exists(model_view_path))
                System.IO.File.Delete(model_view_path);

            string controller_path = Path.Combine(ProjectPath, "Controllers\\" + _CreateObject.Name + "Controller.cs");
            if (System.IO.File.Exists(controller_path))
                System.IO.File.Delete(controller_path);

            string ViewPathDirectory = Path.Combine(ProjectPath, "Views\\" + _CreateObject.Name);
            if (Directory.Exists(ViewPathDirectory))
                Directory.Delete(ViewPathDirectory, true);

        }
        private bool IsExists(long id)
        {
            return _context.CreateObject.Any(e => e.Id == id);
        }
    }
}
