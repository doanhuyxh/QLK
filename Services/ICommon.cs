using UAParser;
using AMS.Models;
using AMS.Models.CommonViewModel;
using AMS.Models.UserAccountViewModel;
using AMS.Models.CompanyInfoViewModel;
using Microsoft.AspNetCore.Identity;
using Services;
using SautinSoft.Document;
using System.Drawing;
using System.Collections;
using AMS.Models.QuanLyMenuViewModel;
using AMS.Models.CreateObjectViewModel;

namespace AMS.Services
{
    public interface ICommon
    {
        Task<string> UploadedFile(IFormFile ProfilePicture);
        Task<string> UploadedFilePdf(IFormFile filePdfPath);
        Bitmap SaveFirstPagePdfToImage(string pdfFilepath, string savePath);
        string CaptureContent(IFormFile ProfilePicture);
        DocumentCore CaptureContentPdf(string CaptureContentPdf);
        string GetContentRootPath();
        bool GetPermission(string userName, string Permission);
        string GetFolderUploadPath();
        string GetFolderDocumentPath();
        Task<List<Folder>> TreeDirectoryNest();
        Task<SMTPEmailSetting> GetSMTPEmailSetting();
        Task<SendGridSetting> GetSendGridEmailSetting();
        UserProfile GetByUserProfile(int id);
        UserProfileCRUDViewModel GetByUserProfileInfo(int id);
        Task<bool> InsertLoginHistory(LoginHistory _LoginHistory, ClientInfo _ClientInfo);
        IQueryable<ItemDropdownListViewModel> GetCommonddlData(string strTableName);
        IQueryable<CreateObjectCRUDViewModel> LoadObjectView();
        Dictionary<string, List<KeyValuePair<string, bool>>> LoadTablesName();
        List<String> LoadFieldsByTableName(string tablename);
        IQueryable<QuanLyMenuCRUDViewModel> LoadQuanLyMenuByParentId(int ParentId, int ExcludeId = 0);
        List<QuanLyMenuCRUDViewModel> GetMenuTree(int level = 0, int ParentId = 1);
        void LoadQuanLyMenu(ref List<QuanLyMenuCRUDViewModel> QuanLyMenuCRUDViewModelList, int level = 0, int ParentId = 1, int ExcludeId = 0);

        IQueryable<UserProfileCRUDViewModel> GetUserProfiles();


        CompanyInfoCRUDViewModel GetCompanyInfo();
        Task<UpdateRoleViewModel> GetRoleByUser(string _ApplicationUserId, UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager);
        string GetTextByImage(string pdfFilePath);
        string GetTextByByteImage(byte[] image);
        Bitmap ConvertPDFtoImages(string pdfFilePath, int page_index);
        string GetAppDevPath();
        string GetEnvironment();
        string GetDatabaseName();
        bool CheckPermissionInProfile(string user, string permission);
    }
}
