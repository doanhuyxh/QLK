using AMS.Models;
using AMS.Pages;
using System.Threading.Tasks;

namespace AMS.Services
{
    public interface IRoles
    {
        Task GenerateRolesFromPagesAsync();

        Task AddToRoles(string applicationUserId);
        Task<MainMenuViewModel> RolebaseMenuLoad(ApplicationUser _ApplicationUser);
    }
}
