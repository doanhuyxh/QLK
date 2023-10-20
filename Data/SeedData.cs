using AMS.Helpers;
using AMS.Models;
using AMS.Models.UserAccountViewModel;

namespace AMS.Data
{
    public class SeedData
    {

        public IEnumerable<UserProfileViewModel> GetUserProfileList()
        {
            return new List<UserProfileViewModel>
            {
                new UserProfileViewModel { FirstName = "HR5", LastName = "User", Email = "HR1@gmail.com", PasswordHash = "123", ConfirmPassword = "123", PhoneNumber= StaticData.RandomDigits(11), ProfilePicture = "/images/UserIcon/U1.png", Address = "California", Country = "USA" },
                new UserProfileViewModel { FirstName = "HR4", LastName = "User", Email = "HR2@gmail.com", PasswordHash = "123", ConfirmPassword = "123", PhoneNumber= StaticData.RandomDigits(11), ProfilePicture = "/images/UserIcon/U2.png", Address = "California", Country = "USA" },
                new UserProfileViewModel { FirstName = "HR3", LastName = "User", Email = "HR3@gmail.com", PasswordHash = "123", ConfirmPassword = "123", PhoneNumber= StaticData.RandomDigits(11), ProfilePicture = "/images/UserIcon/U3.png", Address = "California", Country = "USA" },
                new UserProfileViewModel { FirstName = "HR2", LastName = "User", Email = "HR4@gmail.com", PasswordHash = "123", ConfirmPassword = "123", PhoneNumber= StaticData.RandomDigits(11), ProfilePicture = "/images/UserIcon/U4.png", Address = "California", Country = "USA" },
                new UserProfileViewModel { FirstName = "HR1", LastName = "User", Email = "HR5@gmail.com", PasswordHash = "123", ConfirmPassword = "123", PhoneNumber= StaticData.RandomDigits(11), ProfilePicture = "/images/UserIcon/U5.png", Address = "California", Country = "USA" },

                new UserProfileViewModel { FirstName = "Acc1", LastName = "User", Email = "accountants1@gmail.com", PasswordHash = "123", ConfirmPassword = "123", PhoneNumber= StaticData.RandomDigits(11), ProfilePicture = "/images/UserIcon/U6.png", Address = "California", Country = "USA" },
                new UserProfileViewModel { FirstName = "Acc2", LastName = "User", Email = "accountants2@gmail.com", PasswordHash = "123", ConfirmPassword = "123", PhoneNumber= StaticData.RandomDigits(11), ProfilePicture = "/images/UserIcon/U7.png", Address = "California", Country = "USA" },
                new UserProfileViewModel { FirstName = "Acc3", LastName = "User", Email = "accountants3@gmail.com", PasswordHash = "123", ConfirmPassword = "123", PhoneNumber= StaticData.RandomDigits(11), ProfilePicture = "/images/UserIcon/U8.png", Address = "California", Country = "USA" },
                new UserProfileViewModel { FirstName = "Acc4", LastName = "User", Email = "accountants4@gmail.com", PasswordHash = "123", ConfirmPassword = "123", PhoneNumber= StaticData.RandomDigits(11), ProfilePicture = "/images/UserIcon/U9.png", Address = "California", Country = "USA" },
                new UserProfileViewModel { FirstName = "Acc5", LastName = "User", Email = "accountants5@gmail.com", PasswordHash = "123", ConfirmPassword = "123", PhoneNumber= StaticData.RandomDigits(11), ProfilePicture = "/images/UserIcon/U10.png", Address = "California", Country = "USA" },
            };
        }
        public CompanyInfo GetCompanyInfo()
        {
            return new CompanyInfo
            {
                Name = "XYZ Company Limited",
                Logo = "/upload/company_logo.png",
                Currency = "৳",
                Address = "Dhaka, Bangladesh",
                City = "Dhaka",
                Country = "Bangladesh",
                Phone = "132546789",
                Fax = "9999",
                Website = "www.wyx.com",
            };
        }

        public void SeedTable(ApplicationDbContext _context)
        {

        }
    }
}
