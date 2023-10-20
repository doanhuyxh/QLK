using AMS.Models;
using AMS.Models.AccountViewModels;
using AMS.Models.UserAccountViewModel;
using Microsoft.AspNetCore.Identity;



namespace AMS.ConHelper
{
    public interface IAccount
    {
        Task<Tuple<ApplicationUser, IdentityResult>> CreateUserAccount(CreateUserAccountViewModel _CreateUserAccountViewModel);
        Task<Tuple<ApplicationUser, string>> CreateUserProfile(UserProfileViewModel vm, string LoginUser);
    }
}
