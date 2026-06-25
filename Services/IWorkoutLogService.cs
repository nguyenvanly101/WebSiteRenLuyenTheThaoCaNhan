using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Services
{
    public interface IWorkoutLogService
    {
        Task<List<WorkoutLog>> GetUserLogsAsync(int userId);
        Task<WorkoutLog?> GetLogDetailsAsync(int logId, int userId);
        Task<WorkoutLog?> GetLogByIdAsync(int id, int userId);
        Task<WorkoutLog?> LogDayAsync(int dayId, int userId);
        Task<(bool Success, int LogId)> CreateLogAsync(int userId, WorkoutLogFormViewModel model);
        Task<bool> UpdateLogAsync(int id, int userId, WorkoutLogFormViewModel model);
        Task<bool> DeleteLogAsync(int id, int userId);

        Task<WorkoutLogDetail?> GetDetailByIdAsync(int detailId, int userId);
        Task<bool> AddDetailAsync(int logId, int userId, WorkoutLogDetailFormViewModel model);
        Task<bool> EditDetailAsync(int id, int userId, WorkoutLogDetailFormViewModel model);
        Task<bool> DeleteDetailAsync(int id, int userId);
    }
}
