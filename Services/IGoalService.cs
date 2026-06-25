using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Services
{
    public interface IGoalService
    {
        Task<List<Goal>> GetUserGoalsAsync(int userId);
        Task<Goal?> GetGoalByIdAsync(int id, int userId);
        Task<bool> CreateGoalAsync(int userId, GoalFormViewModel model);
        Task<bool> UpdateGoalAsync(int id, int userId, GoalFormViewModel model);
        Task<bool> DeleteGoalAsync(int id, int userId);
    }
}
