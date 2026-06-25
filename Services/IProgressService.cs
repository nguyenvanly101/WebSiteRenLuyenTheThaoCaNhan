using System.Threading.Tasks;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Services
{
    public interface IProgressService
    {
        Task<DashboardViewModel> GetDashboardDataAsync(int userId);
        Task<ProgressViewModel> GetProgressDataAsync(int userId);
    }
}
