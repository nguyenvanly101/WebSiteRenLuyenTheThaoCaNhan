using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Services
{
    public interface IAdminService
    {
        Task<LandingPageViewModel> GetLandingPageStatsAsync();
        Task<AdminDashboardViewModel> GetAdminDashboardStatsAsync();
        Task<List<AdminUserListItemViewModel>> GetUsersListAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> UpdateUserAsync(int id, AdminUserEditViewModel model, int currentAdminId);
        Task<(bool Success, string? ErrorMessage)> DeleteUserAsync(int id, int currentAdminId);
    }
}
