using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebsiteRenLuyenTheThaoCaNhan.Data;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Services
{
    public class ProgressService : IProgressService
    {
        private readonly ApplicationDbContext _context;

        public ProgressService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(int userId)
        {
            var user = await _context.Users.FirstAsync(item => item.UserID == userId);
            
            var plans = await _context.WorkoutPlans
                .Where(item => item.UserID == userId)
                .Include(item => item.WorkoutDays)
                .ThenInclude(day => day.WorkoutExercises)
                .AsSplitQuery()
                .OrderByDescending(item => item.CreatedAt)
                .Take(4)
                .ToListAsync();

            var goals = await _context.Goals
                .Where(item => item.UserID == userId)
                .OrderBy(item => item.EndDate)
                .ToListAsync();

            var logs = await _context.WorkoutLogs
                .Where(item => item.UserID == userId)
                .Include(item => item.WorkoutLogDetails)
                .OrderByDescending(item => item.WorkoutDate)
                .ToListAsync();

            var weekStart = DateTime.Today.AddDays(-6);
            var weeklyWorkouts = Enumerable.Range(0, 7)
                .Select(offset =>
                {
                    var date = weekStart.AddDays(offset);
                    var value = logs.Count(log => log.WorkoutDate.Date == date);
                    return new BarChartItemViewModel
                    {
                        Label = date.ToString("dd/MM"),
                        Value = value,
                        MaxValue = 1,
                        Hint = value == 0 ? "Không có buổi tập" : $"{value} buổi tập"
                    };
                })
                .ToList();

            var maxWeekValue = Math.Max(weeklyWorkouts.Max(item => item.Value), 1);
            foreach (var item in weeklyWorkouts)
            {
                item.MaxValue = maxWeekValue;
            }

            var goalItems = goals.Select(goal => new GoalProgressItemViewModel
            {
                GoalId = goal.GoalID,
                Title = goal.Title,
                GoalType = goal.GoalType,
                Status = goal.Status,
                CurrentValue = goal.CurrentValue,
                TargetValue = goal.TargetValue,
                Unit = goal.Unit,
                Percent = goal.TargetValue <= 0 ? 0 : (int)Math.Clamp((goal.CurrentValue / goal.TargetValue) * 100, 0, 100)
            }).ToList();

            return new DashboardViewModel
            {
                FullName = user.FullName,
                ActivePlans = plans.Count,
                CompletedWorkouts = logs.Count,
                ActiveGoals = goals.Count(item => item.Status == "Active"),
                GoalCompletionRate = goals.Count == 0
                    ? 0
                    : (int)Math.Round(goals.Count(item => item.Status == "Completed") * 100d / goals.Count),
                Plans = plans.Select(plan => new PlanOverviewItemViewModel
                {
                    PlanId = plan.PlanID,
                    PlanName = plan.PlanName,
                    Goal = plan.Goal,
                    Level = plan.Level,
                    Duration = plan.Duration,
                    DayCount = plan.WorkoutDays.Count,
                    ExerciseCount = plan.WorkoutDays.SelectMany(day => day.WorkoutExercises).Count()
                }).ToList(),
                Goals = goalItems,
                RecentWorkouts = logs.Take(5).Select(log => new RecentWorkoutViewModel
                {
                    LogId = log.LogID,
                    WorkoutDate = log.WorkoutDate,
                    DurationMinutes = log.DurationMinutes,
                    EnergyLevel = log.EnergyLevel,
                    DetailCount = log.WorkoutLogDetails.Count,
                    Note = log.Note
                }).ToList(),
                WeeklyWorkouts = weeklyWorkouts
            };
        }

        public async Task<ProgressViewModel> GetProgressDataAsync(int userId)
        {
            var user = await _context.Users.FirstAsync(item => item.UserID == userId);
            
            var goals = await _context.Goals
                .Where(item => item.UserID == userId)
                .OrderBy(item => item.EndDate)
                .ToListAsync();

            var logs = await _context.WorkoutLogs
                .Where(item => item.UserID == userId)
                .Include(item => item.WorkoutLogDetails)
                .OrderByDescending(item => item.WorkoutDate)
                .ToListAsync();

            var logDetails = await _context.WorkoutLogDetails
                .Where(item => item.WorkoutLog.UserID == userId)
                .Include(item => item.Exercise)
                .ToListAsync();

            var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var monthlyItems = Enumerable.Range(0, 6)
                .Select(offset =>
                {
                    var date = monthStart.AddMonths(offset - 5);
                    var value = logs.Count(log => log.WorkoutDate.Year == date.Year && log.WorkoutDate.Month == date.Month);
                    return new BarChartItemViewModel
                    {
                        Label = date.ToString("MM/yyyy"),
                        Value = value,
                        MaxValue = 1,
                        Hint = value == 0 ? "Chưa có dữ liệu" : $"{value} buổi tập"
                    };
                })
                .ToList();

            var maxMonthValue = Math.Max(monthlyItems.Max(item => item.Value), 1);
            foreach (var item in monthlyItems)
            {
                item.MaxValue = maxMonthValue;
            }

            var muscleGroups = logDetails
                .GroupBy(item => item.Exercise.MuscleGroup)
                .Select(group => new BarChartItemViewModel
                {
                    Label = group.Key,
                    Value = group.Count(),
                    MaxValue = 1,
                    Hint = $"{group.Count()} lần ghi nhận"
                })
                .OrderByDescending(item => item.Value)
                .Take(5)
                .ToList();

            var maxMuscleValue = muscleGroups.Count == 0 ? 1 : Math.Max(muscleGroups.Max(item => item.Value), 1);
            foreach (var item in muscleGroups)
            {
                item.MaxValue = maxMuscleValue;
            }

            return new ProgressViewModel
            {
                FullName = user.FullName,
                TotalSessions = logs.Count,
                SessionsThisMonth = logs.Count(item => item.WorkoutDate.Year == monthStart.Year && item.WorkoutDate.Month == monthStart.Month),
                TotalVolume = logDetails.Sum(item => item.Reps * item.Weight),
                GoalCompletionRate = goals.Count == 0 
                    ? 0 
                    : (int)Math.Round(goals.Count(item => item.Status == "Completed") * 100d / goals.Count),
                GoalProgress = goals.Select(goal => new GoalProgressItemViewModel
                {
                    GoalId = goal.GoalID,
                    Title = goal.Title,
                    GoalType = goal.GoalType,
                    Status = goal.Status,
                    CurrentValue = goal.CurrentValue,
                    TargetValue = goal.TargetValue,
                    Unit = goal.Unit,
                    Percent = goal.TargetValue <= 0 ? 0 : (int)Math.Clamp((goal.CurrentValue / goal.TargetValue) * 100, 0, 100)
                }).ToList(),
                MonthlyWorkouts = monthlyItems,
                MuscleGroups = muscleGroups,
                RecentWorkouts = logs.Take(5).Select(log => new RecentWorkoutViewModel
                {
                    LogId = log.LogID,
                    WorkoutDate = log.WorkoutDate,
                    DurationMinutes = log.DurationMinutes,
                    EnergyLevel = log.EnergyLevel,
                    DetailCount = log.WorkoutLogDetails.Count,
                    Note = log.Note
                }).ToList()
            };
        }
    }
}
