using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Services
{
    public interface IWorkoutPlanService
    {
        Task<List<WorkoutPlan>> GetUserPlansAsync(int userId);
        Task<WorkoutPlan?> GetPlanDetailsAsync(int planId, int userId);
        Task<WorkoutPlan?> GetPlanByIdAsync(int id, int userId);
        Task<(bool Success, int PlanId)> CreatePlanAsync(int userId, WorkoutPlanFormViewModel model);
        Task<bool> UpdatePlanAsync(int id, int userId, WorkoutPlanFormViewModel model);
        Task<bool> DeletePlanAsync(int id, int userId);

        Task<WorkoutDay?> GetDayByIdAsync(int dayId, int userId);
        Task<bool> AddDayAsync(int planId, int userId, WorkoutDayFormViewModel model);
        Task<bool> EditDayAsync(int dayId, int userId, WorkoutDayFormViewModel model);
        Task<bool> DeleteDayAsync(int dayId, int userId);
        Task<bool> DayNumberExistsAsync(int planId, int dayNumber, int? excludeDayId = null);

        Task<WorkoutExercise?> GetWorkoutExerciseByIdAsync(int id, int userId);
        Task<bool> AddExerciseAsync(int dayId, int userId, WorkoutExerciseFormViewModel model);
        Task<bool> EditExerciseAsync(int id, int userId, WorkoutExerciseFormViewModel model);
        Task<bool> DeleteExerciseAsync(int id, int userId);
    }
}
