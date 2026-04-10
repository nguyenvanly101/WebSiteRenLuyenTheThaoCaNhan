using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteRenLuyenTheThaoCaNhan.Data;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers;

[Authorize]
public class ProgressController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProgressController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _context.Users.FirstAsync(item => item.UserID == userId.Value);
        var goals = await _context.Goals.Where(item => item.UserID == userId.Value).OrderBy(item => item.EndDate).ToListAsync();
        var logs = await _context.WorkoutLogs
            .Where(item => item.UserID == userId.Value)
            .Include(item => item.WorkoutLogDetails)
            .OrderByDescending(item => item.WorkoutDate)
            .ToListAsync();
        var logDetails = await _context.WorkoutLogDetails
            .Where(item => item.WorkoutLog.UserID == userId.Value)
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
                    Hint = value == 0 ? "Chua co du lieu" : $"{value} buoi tap"
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
                Hint = $"{group.Count()} lan ghi nhan"
            })
            .OrderByDescending(item => item.Value)
            .Take(5)
            .ToList();

        var maxMuscleValue = muscleGroups.Count == 0 ? 1 : Math.Max(muscleGroups.Max(item => item.Value), 1);
        foreach (var item in muscleGroups)
        {
            item.MaxValue = maxMuscleValue;
        }

        var model = new ProgressViewModel
        {
            FullName = user.FullName,
            TotalSessions = logs.Count,
            SessionsThisMonth = logs.Count(item => item.WorkoutDate.Year == monthStart.Year && item.WorkoutDate.Month == monthStart.Month),
            TotalVolume = logDetails.Sum(item => item.Reps * item.Weight),
            GoalCompletionRate = goals.Count == 0 ? 0 : (int)Math.Round(goals.Count(item => item.Status == "Completed") * 100d / goals.Count),
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

        return View(model);
    }
}
