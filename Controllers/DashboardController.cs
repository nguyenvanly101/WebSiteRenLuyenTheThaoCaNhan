using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteRenLuyenTheThaoCaNhan.Data;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.ViewModels;

namespace WebsiteRenLuyenTheThaoCaNhan.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (User.IsInRole(AppRoles.Admin))
        {
            return RedirectToAction("Index", "Admin");
        }

        var userId = User.GetUserId();
        if (userId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _context.Users.FirstAsync(item => item.UserID == userId.Value);
        var plans = await _context.WorkoutPlans
            .Where(item => item.UserID == userId.Value)
            .Include(item => item.WorkoutDays)
            .ThenInclude(day => day.WorkoutExercises)
            .AsSplitQuery()
            .OrderByDescending(item => item.CreatedAt)
            .Take(4)
            .ToListAsync();

        var goals = await _context.Goals
            .Where(item => item.UserID == userId.Value)
            .OrderBy(item => item.EndDate)
            .ToListAsync();

        var logs = await _context.WorkoutLogs
            .Where(item => item.UserID == userId.Value)
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
                    Hint = value == 0 ? "Khong co buoi tap" : $"{value} buoi tap"
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

        var model = new DashboardViewModel
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

        return View(model);
    }
}
