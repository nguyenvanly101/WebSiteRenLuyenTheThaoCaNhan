using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebsiteRenLuyenTheThaoCaNhan.Data;
using WebsiteRenLuyenTheThaoCaNhan.Models;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly ApplicationDbContext _context;

        public ExerciseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminExerciseListItemViewModel>> GetAdminExercisesAsync()
        {
            return await _context.Exercises
                .OrderBy(item => item.Name)
                .Select(item => new AdminExerciseListItemViewModel
                {
                    Exercise = item,
                    PlanUsageCount = item.WorkoutExercises.Count,
                    LogUsageCount = item.WorkoutLogDetails.Count
                })
                .ToListAsync();
        }

        public async Task<List<Exercise>> GetAllExercisesAsync()
        {
            return await _context.Exercises.OrderBy(item => item.Name).ToListAsync();
        }

        public async Task<Exercise?> GetByIdAsync(int id)
        {
            return await _context.Exercises.FindAsync(id);
        }

        public async Task<bool> CreateAsync(Exercise exercise)
        {
            exercise.CreatedAt = DateTime.UtcNow;
            _context.Exercises.Add(exercise);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(int id, Exercise updatedExercise)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise is null)
            {
                return false;
            }

            exercise.Name = updatedExercise.Name.Trim();
            exercise.MuscleGroup = updatedExercise.MuscleGroup.Trim();
            exercise.Equipment = updatedExercise.Equipment.Trim();
            exercise.Difficulty = updatedExercise.Difficulty.Trim();
            exercise.Description = updatedExercise.Description.Trim();
            exercise.VideoUrl = updatedExercise.VideoUrl?.Trim() ?? string.Empty;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise is null)
            {
                return false;
            }

            _context.Exercises.Remove(exercise);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
