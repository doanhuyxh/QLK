using Microsoft.EntityFrameworkCore;
using System.Net;
using UAParser;
using AMS.Data;
using AMS.Models;
using AMS.Models.CommonViewModel;

using AMS.Models.UserAccountViewModel;

using AMS.Models.CompanyInfoViewModel;
using Microsoft.AspNetCore.Identity;
using Tesseract;
using Ardalis.Extensions.StringManipulation;
using System.Drawing;
using SautinSoft.Document;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Exporting;
using Syncfusion.Pdf;
using System.Drawing.Imaging;
using Services;
using Windows.UI.Xaml.Shapes;
using Path = System.IO.Path;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using Syncfusion.Pdf.ColorSpace;
using BitMiracle.Docotic.Pdf;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq.Expressions;
using GroupDocs.Merger;
using Nest;
using System.IO;
using AMS.Models.QuanLyMenuViewModel;
using System.Reflection.Metadata.Ecma335;
using System.Collections;
using static AMS.Pages.MainMenu;
using System.Globalization;
using UserProfile = AMS.Models.UserProfile;
using LoginHistory = AMS.Models.LoginHistory;
using AMS.Helpers;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using AMS.Models.CreateObjectViewModel;
using Newtonsoft.Json;

namespace AMS.Services
{
    public class Common : ICommon
    {
        private readonly IWebHostEnvironment _iHostingEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public Common(IWebHostEnvironment iHostingEnvironment, ApplicationDbContext context, IConfiguration configuration)
        {
            _iHostingEnvironment = iHostingEnvironment;
            _context = context;
            _configuration = configuration;
        }
        public string GetContentRootPath()
        {
            return _iHostingEnvironment.ContentRootPath;
        }
        public string GetFolderUploadPath()
        {
            return Path.Combine(_iHostingEnvironment.ContentRootPath, "wwwroot/upload");
        }
        public string GetFolderDocumentPath()
        {
            CompanyInfoCRUDViewModel companyInfo = GetCompanyInfo();
            return companyInfo.DocumentPath != "" ? companyInfo.DocumentPath : Path.Combine(GetFolderUploadPath(), "wwwroot/upload/Documents/");
        }
        public Bitmap SaveFirstPagePdfToImage(string pdfFilepath, string savePath)
        {
            Bitmap bitmap = ConvertPDFtoImages(pdfFilepath, 0);
            bitmap.Save(savePath);
            return bitmap;
        }
        public async Task<string> UploadedFile(IFormFile ProfilePicture)
        {
            string ProfilePictureFileName = null;

            if (ProfilePicture != null)
            {
                string uploadsFolder = Path.Combine(_iHostingEnvironment.ContentRootPath, "wwwroot/upload");

                if (ProfilePicture.FileName == null)
                    ProfilePictureFileName = Guid.NewGuid().ToString() + "_" + "blank-person.png";
                else
                    ProfilePictureFileName = Guid.NewGuid().ToString() + "_" + ProfilePicture.FileName;
                string filePath = Path.Combine(uploadsFolder, ProfilePictureFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilePicture.CopyTo(fileStream);
                }
            }
            return ProfilePictureFileName;
        }

        public async Task<string> UploadedFilePdf(IFormFile filePdfPath)
        {
            string PdfFileName = null;

            if (filePdfPath != null)
            {
                string uploadsFolder = Path.Combine(_iHostingEnvironment.ContentRootPath, "wwwroot/upload/Documents/UploadPdf");

                if (filePdfPath.FileName == null)
                    return null;
                else
                    PdfFileName = Guid.NewGuid().ToString() + "_" + filePdfPath.FileName;
                string filePath = Path.Combine(uploadsFolder, PdfFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    filePdfPath.CopyTo(fileStream);
                }
            }
            return PdfFileName;
        }
        public async Task<List<Folder>> TreeDirectoryNest()
        {
            string rootFolder = GetFolderDocumentPath();
            // Get all subdirectories

            string[] subdirectoryEntries = Directory.GetDirectories(rootFolder);

            // Loop through them to see if they have any other subdirectories
            List<Folder> ListFolder = new List<Folder>();
            foreach (string subdirectory in subdirectoryEntries)
            {
                DirectoryInfo dir_info = new DirectoryInfo(subdirectory);
                string _path = subdirectory.Replace(rootFolder, "");
                Folder folder = new Folder { id = _path, text = dir_info.Name, _path = _path };
                folder.children = await LoadSubDirs(subdirectory, rootFolder);
                ListFolder.Add(folder);
            }
            return ListFolder;
        }
        public async Task<List<Folder>> LoadSubDirs(string dir, string rootFolder)
        {
            string[] subdirectoryEntries = Directory.GetDirectories(dir);
            List<Folder> ListFolder = new List<Folder>();
            foreach (string subdirectory in subdirectoryEntries)
            {
                DirectoryInfo dir_info = new DirectoryInfo(subdirectory);
                string _path = subdirectory.Replace(rootFolder, "");
                Folder folder = new Folder { id = _path, text = dir_info.Name, _path = _path }; //LoadSubDirs(subdirectory, folder1);
                folder.children = await LoadSubDirs(subdirectory, rootFolder);
                ListFolder.Add(folder);
            }
            return ListFolder;
        }

        public string CaptureContent(IFormFile ProfilePicture)
        {
            string ProfilePictureFileName = null;
            var text = "";
            if (ProfilePicture != null)
            {
                string uploadsFolder = Path.Combine(_iHostingEnvironment.ContentRootPath, "wwwroot/upload");
                ProfilePictureFileName = Guid.NewGuid().ToString() + "_" + ProfilePicture.FileName;


                string filePath = Path.Combine(uploadsFolder, ProfilePictureFileName);
                string fileDocPathOutPut = Path.Combine(uploadsFolder, Path.GetFileNameWithoutExtension(ProfilePictureFileName) + ".doc");
                Image tempImage = null;

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilePicture.CopyTo(fileStream);
                    fileStream.Close();
                    text = LoadScannedPdf(filePath, fileDocPathOutPut);
                    File.Delete(filePath);
                    // = Image.FromStream(fileStream);
                    //tempImage.Save(System.IO.Path.GetDirectoryName(filePath) + "\\" + System.IO.Path.GetFileNameWithoutExtension(filePath) + ".tif", System.Drawing.Imaging.ImageFormat.Tiff);
                    //text = tttt(System.IO.Path.GetDirectoryName(filePath) + "\\" + System.IO.Path.GetFileNameWithoutExtension(filePath) + ".tif");
                }

            }
            return text;
        }
        public DocumentCore CaptureContentPdf(string PdfFilePath)
        {
            DocumentCore documentCore = null;
            if (PdfFilePath != null)
            {
                PdfLoadOptions lo = new PdfLoadOptions();
                lo.OCROptions.OCRMode = OCRMode.Enabled;
                lo.PreserveEmbeddedFonts = PropertyState.Auto;

                // You can specify all Tesseract parameters inside the method PerformOCR.
                lo.OCROptions.Method = PerformOCRTesseract;
                documentCore = DocumentCore.Load(PdfFilePath, lo);
                // Make all text visible after Tesseract OCR (change font color to Black).
                // The matter is that Tesseract returns OCR result PDF document with invisible text.
                // But with help of Document .Net, we can change the text color,
                // char scaling and spacing to desired.
                foreach (Run r in documentCore.GetChildElements(true, ElementType.Run))
                {
                    r.CharacterFormat.FontColor = SautinSoft.Document.Color.Black;
                    r.CharacterFormat.Scaling = 100;
                    r.CharacterFormat.Spacing = 0;
                }

            }
            return documentCore;
        }
        public Bitmap ConvertPDFtoImages(string pdfPath, int page_index = 0)
        {
            // Path to a document where to extract pictures.
            // By the way: You may specify DOCX, HTML, RTF files also.
            Stream fileStream = File.OpenRead(pdfPath);
            DocumentCore dc = DocumentCore.Load(fileStream, LoadOptions.PdfDefault);

            // PaginationOptions allow to know, how many pages we have in the document.
            DocumentPaginator dp = dc.GetPaginator(new PaginatorOptions());

            // Each document page will be saved in its own image format: PNG, JPEG, TIFF with different DPI.
            return dp.Pages[page_index].Rasterize(100, SautinSoft.Document.Color.White);

        }
        public string LoadScannedPdf(string inpFile, string outFile)
        {
            // Here we'll load a scanned PDF document (perform OCR) containing a text on English, Russian and Vietnamese.
            // Next save the OCR result as a new DOCX document.

            // First steps:

            // 1. Download data files for English, Russian and Vietnamese languages.
            // Please download the files: eng.traineddata, rus.traineddata and vie.traineddata.
            // From here (good and fast): https://github.com/tesseract-ocr/tessdata_fast
            // or (best and slow): https://github.com/tesseract-ocr/tessdata_best

            // 2. Copy the files: eng.traineddata, rus.traineddata and vie.traineddata to
            // the folder "tessdata" in the Project root.

            // 3. Be sure that the folder "tessdata" also contains "pdf.ttf" file.

            // Let's start:

            PdfLoadOptions lo = new PdfLoadOptions();
            lo.OCROptions.OCRMode = OCRMode.Enabled;
            lo.PreserveEmbeddedFonts = PropertyState.Auto;

            // You can specify all Tesseract parameters inside the method PerformOCR.
            lo.OCROptions.Method = PerformOCRTesseract;
            DocumentCore dc = DocumentCore.Load(inpFile, lo);

            // Make all text visible after Tesseract OCR (change font color to Black).
            // The matter is that Tesseract returns OCR result PDF document with invisible text.
            // But with help of Document .Net, we can change the text color,
            // char scaling and spacing to desired.
            foreach (Run r in dc.GetChildElements(true, ElementType.Run))
            {
                r.CharacterFormat.FontColor = SautinSoft.Document.Color.Black;
                r.CharacterFormat.Scaling = 100;
                r.CharacterFormat.Spacing = 0;
            }

            dc.Save(outFile);


            // Open the result for demonstration purposes.
            //System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(outFile) { UseShellExecute = true });
            return dc.Content.ToString();
        }
        public byte[] PerformOCRTesseract(byte[] image)
        {
            // Specify that Tesseract use three 3 languages: English, Russian and Vietnamese.
            //string tesseractLanguages = "rus+eng+vie";
            string tesseractLanguages = "eng+vie";

            // A path to a folder which contains languages data files and font file "pdf.ttf".
            // Language data files can be found here:
            // Good and fast: https://github.com/tesseract-ocr/tessdata_fast
            // or
            // Best and slow: https://github.com/tesseract-ocr/tessdata_best
            // Also this folder must have write permissions.
            string tesseractData = Path.Combine(_iHostingEnvironment.ContentRootPath, "wwwroot/tessdata");


            // A path for a temporary PDF file (because Tesseract returns OCR result as PDF document)
            string tempFile = Path.Combine(tesseractData, Path.GetRandomFileName());

            try
            {
                using (IResultRenderer renderer = PdfResultRenderer.CreatePdfRenderer(tempFile, tesseractData, true))
                {
                    using (renderer.BeginDocument("Serachablepdf"))
                    {
                        using (TesseractEngine engine = new TesseractEngine(tesseractData, tesseractLanguages, Tesseract.EngineMode.Default))
                        {
                            engine.DefaultPageSegMode = PageSegMode.Auto;
                            using (MemoryStream msImg = new MemoryStream(image))
                            {
                                Image imgWithText = Image.FromStream(msImg);
                                for (int i = 0; i < imgWithText.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page); i++)
                                {
                                    imgWithText.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, i);
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        imgWithText.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                        byte[] imgBytes = ms.ToArray();
                                        using (Pix img = Pix.LoadFromMemory(imgBytes))
                                        {
                                            using (var page = engine.Process(img, "Serachablepdf"))
                                            {
                                                renderer.AddPage(page);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return File.ReadAllBytes(tempFile + ".pdf");
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("Please be sure that you have Language data files (*.traineddata) in your folder \"tessdata\"");
                Console.WriteLine("The Language data files can be download from here: https://github.com/tesseract-ocr/tessdata_fast");
                Console.ReadKey();
                throw new Exception("Error Tesseract: " + e.Message);
            }
            finally
            {
                if (File.Exists(tempFile + ".pdf"))
                    File.Delete(tempFile + ".pdf");
            }
        }


        public string GetTextByImage(string testImagePath)
        {
            string text = "";
            try
            {
                string filePathTessdata = Path.Combine(_iHostingEnvironment.ContentRootPath, "wwwroot/tessdata");

                using (var engine = new TesseractEngine(filePathTessdata, "vie", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(testImagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            text = page.GetText();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                text = e.Message;
            }
            return text;
        }
        public string GetTextByByteImage(byte[] image)
        {
            string text = "";
            try
            {
                string filePathTessdata = Path.Combine(_iHostingEnvironment.ContentRootPath, "wwwroot/tessdata");

                using (var engine = new TesseractEngine(filePathTessdata, "vie", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromMemory(image))
                    {
                        using (var page = engine.Process(img))
                        {
                            text = page.GetText();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                text = e.Message;
            }
            return text;
        }
        public async Task<SMTPEmailSetting> GetSMTPEmailSetting()
        {
            return await _context.Set<SMTPEmailSetting>().Where(x => x.Id == 1).SingleOrDefaultAsync();
        }
        public async Task<SendGridSetting> GetSendGridEmailSetting()
        {
            return await _context.Set<SendGridSetting>().Where(x => x.Id == 1).SingleOrDefaultAsync();
        }

        public UserProfile GetByUserProfile(int id)
        {
            return _context.UserProfile.Where(x => x.UserProfileId == id).SingleOrDefault();
        }
        public UserProfileCRUDViewModel GetByUserProfileInfo(int id)
        {
            UserProfileCRUDViewModel _UserProfileCRUDViewModel = _context.UserProfile.Where(x => x.UserProfileId == id).SingleOrDefault();
            return _UserProfileCRUDViewModel;
        }
        public async Task<bool> InsertLoginHistory(LoginHistory _LoginHistory, ClientInfo _ClientInfo)
        {
            try
            {
                _LoginHistory.PublicIP = GetPublicIP();
                _LoginHistory.CreatedDate = DateTime.Now;
                _LoginHistory.ModifiedDate = DateTime.Now;

                _context.Add(_LoginHistory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetPublicIP()
        {
            try
            {
                string url = "http://checkip.dyndns.org/";
                WebRequest req = WebRequest.Create(url);
                WebResponse resp = req.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream());
                string response = sr.ReadToEnd().Trim();
                string[] a = response.Split(':');
                string a2 = a[1].Substring(1);
                string[] a3 = a2.Split('<');
                string a4 = a3[0];
                return a4;
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw new Exception("No network adapters with an IPv4 address in the system!");
            }
        }
        public IQueryable<ItemDropdownListViewModel> GetCommonddlData(string strTableName)
        {
            var sql = "select Id, Name from " + strTableName + " where Cancelled = 0";
            var result = _context.ItemDropdownListViewModel.FromSqlRaw(sql);
            return result;
        }
        public IQueryable<CreateObjectCRUDViewModel> LoadObjectView()
        {
            return (from tblObj in _context.CreateObject.Where(x => x.Cancelled == false).OrderBy(x => x.Id)
                    select new CreateObjectCRUDViewModel
                    {
                        Id = tblObj.Id,
                        Name = tblObj.Name,
                        Label = tblObj.Label,
                        IsTemplate = tblObj.IsTemplate,
                    });
        }
        public Dictionary<string, List<KeyValuePair<string, bool>>> LoadTablesName()
        {
            Dictionary<string, List<KeyValuePair<string, bool>>> listTableField = new Dictionary<string, List<KeyValuePair<string, bool>>>();
            string sql = $"SELECT TABLE_NAME,COLUMN_Name FROM INFORMATION_SCHEMA.COLUMNS";

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                _context.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {

                    while (result.Read())
                    {
                        var result0 = result[0].ToString();
                        var result1 = result[1].ToString();
                        if (!listTableField.ContainsKey(result0))
                        {
                            listTableField.Add(result0, new List<KeyValuePair<string, bool>>());
                        }
                        listTableField[result0].Add(new KeyValuePair<string, bool>(result1, false));
                    }
                }
            }

            return listTableField;
        }
        public List<String> LoadFieldsByTableName(string tablename)
        {
            List<String> ListTable = new List<string>();
            var sql = $"SELECT COLUMN_Name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{tablename}'";

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                _context.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {

                    while (result.Read())
                    {
                        ListTable.Add(result[0].ToString());
                    }


                }
            }
            return ListTable;
        }

        public IQueryable<QuanLyMenuCRUDViewModel> LoadQuanLyMenuByParentId(int ParentId, int ExcludeId = 0)
        {
            try
            {
                return (
                from _QuanLyMenu in _context.QuanLyMenu.Where(x => x.ParentId == ParentId && x.Id != ExcludeId).OrderBy(x => x.Ordering)
                join _CreateObject in _context.CreateObject on _QuanLyMenu.CreateObjectId equals _CreateObject.Id
                   into listCreateObject
                from _CreateObject in listCreateObject.DefaultIfEmpty()
                select new QuanLyMenuCRUDViewModel
                {
                    Id = _QuanLyMenu.Id,
                    ParentId = _QuanLyMenu.ParentId,
                    Ordering = _QuanLyMenu.Ordering,
                    Description = _QuanLyMenu.Description,
                    Parameter = _QuanLyMenu.Parameter,
                    View = _CreateObject.Name,
                    Icon = _QuanLyMenu.Icon,
                    DevelopmentEnvironment = _QuanLyMenu.DevelopmentEnvironment,
                    Name = _QuanLyMenu.Name,
                    Label = _QuanLyMenu.Name,
                    Task = _QuanLyMenu.Task.Trim(),
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void LoadQuanLyMenu(ref List<QuanLyMenuCRUDViewModel> QuanLyMenuCRUDViewModelList, int level = 0, int ParentId = 1, int ExcludeId = 0)
        {
            try
            {
                IQueryable<QuanLyMenuCRUDViewModel> _QuanLyMenuCRUDViewModelList = LoadQuanLyMenuByParentId(ParentId, ExcludeId);
                var level1 = level + 1;
                if (_QuanLyMenuCRUDViewModelList.Count() > 0)
                {
                    foreach (var item in _QuanLyMenuCRUDViewModelList)
                    {
                        var view = item.View != null ? item.View : "";
                        view = Utility.RemoveUnicode(view);
                        view = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(view.ToLower());
                        view = String.Concat(view.Where(c => !Char.IsWhiteSpace(c)));
                        item.Name = "---".Repeat((uint)level) + $" <i class=\"fas {item.Icon}\" /> " + item.Name;
                        item.View = view;
                        item.TotalItemOfParent = _QuanLyMenuCRUDViewModelList.Count();

                        QuanLyMenuCRUDViewModelList.Add(item);
                        LoadQuanLyMenu(ref QuanLyMenuCRUDViewModelList, level1, item.Id, ExcludeId);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public string GetAppDevPath()
        {
            return _configuration.GetValue<string>("ApplicationInfo:AppDevPath");
        }
        public string GetEnvironment()
        {
            return _iHostingEnvironment.EnvironmentName;
        }

        public string GetDatabaseName()
        {
            return _configuration.GetValue<string>("ApplicationInfo:DatabaseName");
        }
        public List<QuanLyMenuCRUDViewModel> GetMenuTree(int level = 0, int ParentId = 1)
        {
            try
            {
                IQueryable<QuanLyMenuCRUDViewModel> _QuanLyMenuCRUDViewModelList = LoadQuanLyMenuByParentId(ParentId);
                var level1 = level + 1;
                List<QuanLyMenuCRUDViewModel> Children = new List<QuanLyMenuCRUDViewModel>();
                if (_QuanLyMenuCRUDViewModelList != null && _QuanLyMenuCRUDViewModelList.Count() > 0)
                {

                    foreach (var item in _QuanLyMenuCRUDViewModelList)
                    {
                        var view = item.View != null ? item.View : "";
                        view = Utility.RemoveUnicode(view);
                        view = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(view.ToLower());
                        view = String.Concat(view.Where(c => !Char.IsWhiteSpace(c)));
                        item.View = view;
                        item.Parameter = String.IsNullOrEmpty(item.Parameter) ? "" : ("&" + item.Parameter);
                        item.Children = GetMenuTree(level1, item.Id);
                        Children.Add(item);


                    }
                }
                return Children;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public IQueryable<UserProfileCRUDViewModel> GetUserProfiles()
        {
            try
            {
                var result = (from _UserProfile in _context.UserProfile
                              join _QuanLyNhomNguoiDung in _context.QuanLyNhomNguoiDung
                              on _UserProfile.GroupUserId equals _QuanLyNhomNguoiDung.Id
                                into QuanLyNhomNguoiDungList
                              from _QuanLyNhomNguoiDung in QuanLyNhomNguoiDungList.DefaultIfEmpty()
                              where _UserProfile.Cancelled == false
                              select new UserProfileCRUDViewModel
                              {
                                  ApplicationUserId = _UserProfile.ApplicationUserId,
                                  UserProfileId = _UserProfile.UserProfileId,
                                  FirstName = _UserProfile.FirstName,
                                  LastName = _UserProfile.LastName,
                                  PhoneNumber = _UserProfile.PhoneNumber,
                                  Email = _UserProfile.Email,
                                  Address = _UserProfile.Address,
                                  Country = _UserProfile.Country,
                                  GroupUserId = _UserProfile.GroupUserId,

                                  CreatedDate = _UserProfile.CreatedDate,
                                  ModifiedDate = _UserProfile.ModifiedDate,
                                  CreatedBy = _UserProfile.CreatedBy,
                                  ModifiedBy = _UserProfile.ModifiedBy,
                                  ProfilePicturePath = _UserProfile.ProfilePicture,
                                  GroupUserDisplay = _QuanLyNhomNguoiDung.TenNhom
                              }).OrderByDescending(x => x.UserProfileId);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CompanyInfoCRUDViewModel GetCompanyInfo()
        {
            CompanyInfoCRUDViewModel vm = _context.CompanyInfo.FirstOrDefault(m => m.Id == 1);
            return vm;
        }
        /*
         * var userName = context.HttpContext.User.Identity.Name;
         * if(_iCommon.GetPermission(userName,"KeHoachSanXuat.KeHoachSanXuat.list")){
         *      //TODO Some thing
         * }
         *
         */
        public bool GetPermission(string userName, string Permission)
        {
            try
            {
                var GroupUser = (from _Users in _context.Users
                                 join _QuanLyNhomNguoiDung in _context.QuanLyNhomNguoiDung on _Users.GroupUserId equals _QuanLyNhomNguoiDung.Id
                                 where _Users.UserName.Equals(userName)
                                 select new
                                 {
                                     Email = _Users.Email,
                                     GroupUserId = _Users.GroupUserId,
                                     Permistions = _QuanLyNhomNguoiDung.Permistions,
                                 }).FirstOrDefault();
                var requiredPermissions = Permission.Split(".");
                var CurrentPermistions = GroupUser.Permistions;
                List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, Access>>>>>> DyPermistions = JsonConvert.DeserializeObject<List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, Access>>>>>>>(CurrentPermistions);
                var CurrentValue = DyPermistions.FirstOrDefault(item => item.Key == requiredPermissions[0]).Value.FirstOrDefault(item => item.Key == requiredPermissions[1]).Value.FirstOrDefault(item => item.Key == requiredPermissions[2]).Value.CurrentValue;
                if (CurrentValue)
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;

        }
        public async Task<UpdateRoleViewModel> GetRoleByUser(string _ApplicationUserId, UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            UpdateRoleViewModel _UpdateRoleViewModel = new UpdateRoleViewModel();
            List<ManageUserRolesViewModel> list = new List<ManageUserRolesViewModel>();

            _UpdateRoleViewModel.ApplicationUserId = _ApplicationUserId;
            var user = await _userManager.FindByIdAsync(_ApplicationUserId);
            if (user != null)
            {
                foreach (var role in _roleManager.Roles.ToList())
                {
                    var userRolesViewModel = new ManageUserRolesViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name
                    };
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRolesViewModel.Selected = true;
                    }
                    else
                    {
                        userRolesViewModel.Selected = false;
                    }
                    list.Add(userRolesViewModel);
                }
            }

            _UpdateRoleViewModel.listManageUserRolesViewModel = list.OrderBy(x => x.RoleName).ToList();
            return _UpdateRoleViewModel;
        }

        public bool CheckPermissionInProfile(string user, string permission)
        {
            try
            {
                if (user.Equals(_configuration["SuperAdminDefaultOptions:Email"]))
                {
                    return true;
                }
                var GroupUser = (from _Users in _context.UserProfile
                                 join _QuanLyNhomNguoiDung in _context.QuanLyNhomNguoiDung on _Users.GroupUserId equals _QuanLyNhomNguoiDung.Id
                                 where _Users.Cancelled == false && _Users.Email == user
                                 select new
                                 {
                                     Email = _Users.Email,
                                     GroupUserId = _Users.GroupUserId,
                                     Permistions = _QuanLyNhomNguoiDung.Permistions,
                                 }).FirstOrDefault();
                var requiredPermissions = permission.Split(".");
                if (GroupUser == null)
                {
                    return false;
                }
                var CurrentPermistions = GroupUser.Permistions;
                List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, Access>>>>>> DyPermistions = JsonConvert.DeserializeObject<List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, Access>>>>>>>(CurrentPermistions);
                var CurrentValue = DyPermistions.FirstOrDefault(item => item.Key == requiredPermissions[0]).Value.FirstOrDefault(item => item.Key == requiredPermissions[1]).Value.FirstOrDefault(item => item.Key == requiredPermissions[2]).Value.CurrentValue;

                if (CurrentValue)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}