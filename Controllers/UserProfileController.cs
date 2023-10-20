using AMS.Data;
using AMS.Models;
using AMS.Models.UserAccountViewModel;
using AMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class UserProfileController : Controller
    {
        private readonly IRoles _roles;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICommon _iCommon;

        public UserProfileController(UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            IRoles roles,
            ICommon iCommon)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _roles = roles;
            _iCommon = iCommon;
        }

        // [Authorize(Roles = Pages.MainMenu.UserProfile.RoleName)]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var _ApplicationUser = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
            UserProfileCRUDViewModel _UserProfileCRUDViewModel = new UserProfileCRUDViewModel();
            if (_ApplicationUser.Id != null)
            {
                _UserProfileCRUDViewModel = _context.UserProfile.Where(x => x.ApplicationUserId == _ApplicationUser.Id).SingleOrDefault();
            }
            return View(_UserProfileCRUDViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPasswordGeneral(string ApplicationUserId)
        {
            var _ApplicationUser = await _userManager.FindByIdAsync(ApplicationUserId);
            ResetPasswordViewModel _ResetPasswordViewModel = new ResetPasswordViewModel();
            _ResetPasswordViewModel.ApplicationUserId = _ApplicationUser.Id;
            return PartialView("_ResetPasswordGeneral", _ResetPasswordViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordGeneral(ResetPasswordViewModel vm)
        {
            try
            {
                string AlertMessage = string.Empty;
                var _ApplicationUser = await _userManager.FindByIdAsync(vm.ApplicationUserId);
                if (vm.NewPassword.Equals(vm.ConfirmPassword))
                {
                    var result = await _userManager.ChangePasswordAsync(_ApplicationUser, vm.OldPassword, vm.NewPassword);
                    if (result.Succeeded)
                        AlertMessage = "Change Password Succeeded. User name: " + _ApplicationUser.Email;
                    else
                    {
                        string errorMessage = string.Empty;
                        foreach (var item in result.Errors)
                        {
                            errorMessage = errorMessage + " " + item.Description;
                        }
                        AlertMessage = "error" + errorMessage;
                    }
                }
                return new JsonResult(AlertMessage);
            }
            catch (Exception ex)
            {
                return new JsonResult("error" + ex.Message);
                throw ex;
            }
        }
    }
}
