using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Services
{
    public interface IExerciseService
    {
        Task<List<AdminExerciseListItemViewModel>> GetAdminExercisesAsync();
        Task<List<Exercise>> GetAllExercisesAsync();
        Task<Exercise?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Exercise exercise);
        Task<bool> UpdateAsync(int id, Exercise updatedExercise);
        Task<bool> DeleteAsync(int id);
    }
}
