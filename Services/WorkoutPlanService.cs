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
    public class WorkoutPlanService : IWorkoutPlanService
    {
        private readonly ApplicationDbContext _context;

        public WorkoutPlanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkoutPlan>> GetUserPlansAsync(int userId)
        {
            return await _context.WorkoutPlans
                .Where(item => item.UserID == userId)
                .Include(item => item.WorkoutDays)
                .ThenInclude(day => day.WorkoutExercises)
                .AsSplitQuery()
                .OrderByDescending(item => item.CreatedAt)
                .ToListAsync();
        }

        public async Task<WorkoutPlan?> GetPlanDetailsAsync(int planId, int userId)
        {
            return await _context.WorkoutPlans
                .Where(item => item.UserID == userId)
                .Include(item => item.WorkoutDays)
                .ThenInclude(day => day.WorkoutExercises)
                .ThenInclude(item => item.Exercise)
                .AsSplitQuery()
                .FirstOrDefaultAsync(item => item.PlanID == planId);
        }

        public async Task<WorkoutPlan?> GetPlanByIdAsync(int id, int userId)
        {
            return await _context.WorkoutPlans
                .FirstOrDefaultAsync(item => item.PlanID == id && item.UserID == userId);
        }

        public async Task<(bool Success, int PlanId)> CreatePlanAsync(int userId, WorkoutPlanFormViewModel model)
        {
            var plan = new WorkoutPlan
            {
                UserID = userId,
                PlanName = model.PlanName.Trim(),
                Goal = model.Goal.Trim(),
                Level = model.Level.Trim(),
                Summary = model.Summary?.Trim() ?? string.Empty,
                Duration = model.Duration,
                CreatedAt = DateTime.UtcNow
            };

            _context.WorkoutPlans.Add(plan);
            var result = await _context.SaveChangesAsync() > 0;
            return (result, plan.PlanID);
        }

        public async Task<bool> UpdatePlanAsync(int id, int userId, WorkoutPlanFormViewModel model)
        {
            var plan = await _context.WorkoutPlans
                .FirstOrDefaultAsync(item => item.PlanID == id && item.UserID == userId);

            if (plan is null)
            {
                return false;
            }

            plan.PlanName = model.PlanName.Trim();
            plan.Goal = model.Goal.Trim();
            plan.Level = model.Level.Trim();
            plan.Summary = model.Summary?.Trim() ?? string.Empty;
            plan.Duration = model.Duration;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePlanAsync(int id, int userId)
        {
            var plan = await _context.WorkoutPlans
                .FirstOrDefaultAsync(item => item.PlanID == id && item.UserID == userId);

            if (plan is null)
            {
                return false;
            }

            _context.WorkoutPlans.Remove(plan);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<WorkoutDay?> GetDayByIdAsync(int dayId, int userId)
        {
            return await _context.WorkoutDays
                .Include(item => item.WorkoutPlan)
                .FirstOrDefaultAsync(item => item.DayID == dayId && item.WorkoutPlan.UserID == userId);
        }

        public async Task<bool> AddDayAsync(int planId, int userId, WorkoutDayFormViewModel model)
        {
            var plan = await _context.WorkoutPlans
                .FirstOrDefaultAsync(item => item.PlanID == planId && item.UserID == userId);

            if (plan is null)
            {
                return false;
            }

            var day = new WorkoutDay
            {
                PlanID = planId,
                DayNumber = model.DayNumber,
                FocusArea = model.FocusArea.Trim(),
                Note = model.Note?.Trim() ?? string.Empty
            };

            _context.WorkoutDays.Add(day);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EditDayAsync(int dayId, int userId, WorkoutDayFormViewModel model)
        {
            var day = await _context.WorkoutDays
                .Include(item => item.WorkoutPlan)
                .FirstOrDefaultAsync(item => item.DayID == dayId && item.WorkoutPlan.UserID == userId);

            if (day is null)
            {
                return false;
            }

            day.DayNumber = model.DayNumber;
            day.FocusArea = model.FocusArea.Trim();
            day.Note = model.Note?.Trim() ?? string.Empty;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteDayAsync(int dayId, int userId)
        {
            var day = await _context.WorkoutDays
                .Include(item => item.WorkoutPlan)
                .FirstOrDefaultAsync(item => item.DayID == dayId && item.WorkoutPlan.UserID == userId);

            if (day is null)
            {
                return false;
            }

            _context.WorkoutDays.Remove(day);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DayNumberExistsAsync(int planId, int dayNumber, int? excludeDayId = null)
        {
            if (excludeDayId.HasValue)
            {
                return await _context.WorkoutDays
                    .AnyAsync(item => item.PlanID == planId && item.DayNumber == dayNumber && item.DayID != excludeDayId.Value);
            }
            return await _context.WorkoutDays
                .AnyAsync(item => item.PlanID == planId && item.DayNumber == dayNumber);
        }

        public async Task<WorkoutExercise?> GetWorkoutExerciseByIdAsync(int id, int userId)
        {
            return await _context.WorkoutExercises
                .Include(entry => entry.WorkoutDay)
                .ThenInclude(day => day.WorkoutPlan)
                .FirstOrDefaultAsync(entry => entry.ID == id && entry.WorkoutDay.WorkoutPlan.UserID == userId);
        }

        public async Task<bool> AddExerciseAsync(int dayId, int userId, WorkoutExerciseFormViewModel model)
        {
            var day = await _context.WorkoutDays
                .Include(item => item.WorkoutPlan)
                .FirstOrDefaultAsync(item => item.DayID == dayId && item.WorkoutPlan.UserID == userId);

            if (day is null)
            {
                return false;
            }

            var workoutExercise = new WorkoutExercise
            {
                DayID = dayId,
                ExerciseID = model.ExerciseID,
                Sets = model.Sets,
                Reps = model.Reps,
                RestTime = model.RestTime,
                DisplayOrder = model.DisplayOrder
            };

            _context.WorkoutExercises.Add(workoutExercise);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EditExerciseAsync(int id, int userId, WorkoutExerciseFormViewModel model)
        {
            var item = await _context.WorkoutExercises
                .Include(entry => entry.WorkoutDay)
                .ThenInclude(day => day.WorkoutPlan)
                .FirstOrDefaultAsync(entry => entry.ID == id && entry.WorkoutDay.WorkoutPlan.UserID == userId);

            if (item is null)
            {
                return false;
            }

            item.ExerciseID = model.ExerciseID;
            item.Sets = model.Sets;
            item.Reps = model.Reps;
            item.RestTime = model.RestTime;
            item.DisplayOrder = model.DisplayOrder;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteExerciseAsync(int id, int userId)
        {
            var item = await _context.WorkoutExercises
                .Include(entry => entry.WorkoutDay)
                .ThenInclude(day => day.WorkoutPlan)
                .FirstOrDefaultAsync(entry => entry.ID == id && entry.WorkoutDay.WorkoutPlan.UserID == userId);

            if (item is null)
            {
                return false;
            }

            _context.WorkoutExercises.Remove(item);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
