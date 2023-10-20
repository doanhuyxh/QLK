// www.craftedforeveryone.com
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using AMS.Data;
using AMS.Models;
using AMS.Models.QuanLyNhomNguoiDungViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace AMS.Services
{
    //Extenting from AuthorizeAttribute or Attribute is upto user choice.
    //You can consider using AuthorizeAttribute if you want to use the predefined properties and functions from Authorize Attribute.
    public class AuthorizePermission : AuthorizeAttribute, IAuthorizationFilter
    {
        public string Permissions { get; set; } //Permission string to get from controller

        private readonly IConfiguration _configuration;

        public AuthorizePermission(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var applicationDbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
            //Validate if any permissions are passed when using attribute at controller or action level
            if (string.IsNullOrEmpty(Permissions))
            {
                //Validation cannot take place without any permissions so returning unauthorized
                context.Result = new UnauthorizedResult();
                return;
            }


            //Identity.Name will have windows logged in user id, in case of Windows Authentication
            //Indentity.Name will have user name passed from token, in case of JWT Authenntication and having claim type "ClaimTypes.Name"
            var userName = context.HttpContext.User.Identity.Name;
            if (userName == _configuration["SuperAdminDefaultOptions:Email"])
            {
                return;
            }

            try
            {
                var GroupUser = (from _Users in applicationDbContext.UserProfile
                                 join _QuanLyNhomNguoiDung in applicationDbContext.QuanLyNhomNguoiDung on _Users.GroupUserId equals _QuanLyNhomNguoiDung.Id
                                 join _aspUser in applicationDbContext.Users on _Users.ApplicationUserId equals _aspUser.Id
                                 where _aspUser.UserName.Contains(userName)
                              select new
                              {
                                  Email = _Users.Email,
                                  GroupUserId = _Users.GroupUserId,
                                  Permistions = _QuanLyNhomNguoiDung.Permistions,
                              }).FirstOrDefault();
                var requiredPermissions = Permissions.Split(".");
                if (string.IsNullOrEmpty(GroupUser.Permistions))
                {
                    return;
                }
                var CurrentPermistions = GroupUser.Permistions;
                List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, Access>>>>>>  DyPermistions = JsonConvert.DeserializeObject<List<KeyValuePair<string, List<KeyValuePair<string, List<KeyValuePair<string, Access>>>>>>>(CurrentPermistions);
                var CurrentValue = DyPermistions.FirstOrDefault(item => item.Key == requiredPermissions[0]).Value.FirstOrDefault(item => item.Key == requiredPermissions[1]).Value.FirstOrDefault(item => item.Key == requiredPermissions[2]).Value.CurrentValue;
                if (CurrentValue)
                {
                    return;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            context.Result = new UnauthorizedResult();

            return;

        }
    }
}
