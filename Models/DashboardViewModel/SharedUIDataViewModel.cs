using AMS.Models.AccountViewModels;
using AMS.Models.CommonViewModel;
using AMS.Models.QuanLyMenuViewModel;
using AMS.Models.UserAccountViewModel;
using AMS.Pages;

namespace AMS.Models.DashboardViewModel
{
    public class SharedUIDataViewModel
    {
        public UserProfileCRUDViewModel userProfileCRUDViewModel { get; set; }
        public UserProfile UserProfile { get; set; }
        public ApplicationInfo ApplicationInfo { get; set; }
        public MainMenuViewModel MainMenuViewModel { get; set; }
        public LoginViewModel LoginViewModel { get; set; }
        public List<QuanLyMenuCRUDViewModel> MenuTree { get; set; }
        public string Environment { get; set; }
    }
}
