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
    public class WorkoutLogService : IWorkoutLogService
    {
        private readonly ApplicationDbContext _context;

        public WorkoutLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkoutLog>> GetUserLogsAsync(int userId)
        {
            return await _context.WorkoutLogs
                .Where(item => item.UserID == userId)
                .Include(item => item.WorkoutLogDetails)
                .OrderByDescending(item => item.WorkoutDate)
                .ToListAsync();
        }

        public async Task<WorkoutLog?> GetLogDetailsAsync(int logId, int userId)
        {
            return await _context.WorkoutLogs
                .Where(item => item.UserID == userId && item.LogID == logId)
                .Include(item => item.WorkoutLogDetails)
                .ThenInclude(item => item.Exercise)
                .FirstOrDefaultAsync();
        }

        public async Task<WorkoutLog?> GetLogByIdAsync(int id, int userId)
        {
            return await _context.WorkoutLogs
                .FirstOrDefaultAsync(item => item.LogID == id && item.UserID == userId);
        }

        public async Task<WorkoutLog?> LogDayAsync(int dayId, int userId)
        {
            var day = await _context.WorkoutDays
                .Include(item => item.WorkoutPlan)
                .Include(item => item.WorkoutExercises)
                .FirstOrDefaultAsync(item => item.DayID == dayId && item.WorkoutPlan.UserID == userId);

            if (day is null)
            {
                return null;
            }

            var log = new WorkoutLog
            {
                UserID = userId,
                WorkoutDate = DateTime.Today,
                DurationMinutes = 45,
                EnergyLevel = "Balanced",
                Note = $"Ghi nhận tự động từ Ngày {day.DayNumber} ({day.FocusArea}) - Kế hoạch: {day.WorkoutPlan.PlanName}"
            };

            _context.WorkoutLogs.Add(log);
            await _context.SaveChangesAsync();

            foreach (var workoutExercise in day.WorkoutExercises)
            {
                for (int set = 1; set <= workoutExercise.Sets; set++)
                {
                    _context.WorkoutLogDetails.Add(new WorkoutLogDetail
                    {
                        LogID = log.LogID,
                        ExerciseID = workoutExercise.ExerciseID,
                        SetNumber = set,
                        Reps = workoutExercise.Reps,
                        Weight = 0
                    });
                }
            }

            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<(bool Success, int LogId)> CreateLogAsync(int userId, WorkoutLogFormViewModel model)
        {
            var log = new WorkoutLog
            {
                UserID = userId,
                WorkoutDate = model.WorkoutDate,
                DurationMinutes = model.DurationMinutes,
                EnergyLevel = model.EnergyLevel,
                Note = model.Note?.Trim() ?? string.Empty
            };

            _context.WorkoutLogs.Add(log);
            var result = await _context.SaveChangesAsync() > 0;
            return (result, log.LogID);
        }

        public async Task<bool> UpdateLogAsync(int id, int userId, WorkoutLogFormViewModel model)
        {
            var log = await _context.WorkoutLogs
                .FirstOrDefaultAsync(item => item.LogID == id && item.UserID == userId);

            if (log is null)
            {
                return false;
            }

            log.WorkoutDate = model.WorkoutDate;
            log.DurationMinutes = model.DurationMinutes;
            log.EnergyLevel = model.EnergyLevel;
            log.Note = model.Note?.Trim() ?? string.Empty;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteLogAsync(int id, int userId)
        {
            var log = await _context.WorkoutLogs
                .FirstOrDefaultAsync(item => item.LogID == id && item.UserID == userId);

            if (log is null)
            {
                return false;
            }

            _context.WorkoutLogs.Remove(log);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<WorkoutLogDetail?> GetDetailByIdAsync(int detailId, int userId)
        {
            return await _context.WorkoutLogDetails
                .Include(item => item.WorkoutLog)
                .FirstOrDefaultAsync(item => item.ID == detailId && item.WorkoutLog.UserID == userId);
        }

        public async Task<bool> AddDetailAsync(int logId, int userId, WorkoutLogDetailFormViewModel model)
        {
            var log = await _context.WorkoutLogs
                .FirstOrDefaultAsync(item => item.LogID == logId && item.UserID == userId);

            if (log is null)
            {
                return false;
            }

            var detail = new WorkoutLogDetail
            {
                LogID = logId,
                ExerciseID = model.ExerciseID,
                SetNumber = model.SetNumber,
                Reps = model.Reps,
                Weight = model.Weight
            };

            _context.WorkoutLogDetails.Add(detail);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EditDetailAsync(int id, int userId, WorkoutLogDetailFormViewModel model)
        {
            var detail = await _context.WorkoutLogDetails
                .Include(item => item.WorkoutLog)
                .FirstOrDefaultAsync(item => item.ID == id && item.WorkoutLog.UserID == userId);

            if (detail is null)
            {
                return false;
            }

            detail.ExerciseID = model.ExerciseID;
            detail.SetNumber = model.SetNumber;
            detail.Reps = model.Reps;
            detail.Weight = model.Weight;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteDetailAsync(int id, int userId)
        {
            var detail = await _context.WorkoutLogDetails
                .Include(item => item.WorkoutLog)
                .FirstOrDefaultAsync(item => item.ID == id && item.WorkoutLog.UserID == userId);

            if (detail is null)
            {
                return false;
            }

            _context.WorkoutLogDetails.Remove(detail);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
