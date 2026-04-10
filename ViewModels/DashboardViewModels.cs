namespace WebsiteRenLuyenTheThaoCaNhan.ViewModels;

public class DashboardViewModel
{
    public string FullName { get; set; } = string.Empty;
    public int ActivePlans { get; set; }
    public int CompletedWorkouts { get; set; }
    public int ActiveGoals { get; set; }
    public int GoalCompletionRate { get; set; }
    public IReadOnlyList<PlanOverviewItemViewModel> Plans { get; set; } = Array.Empty<PlanOverviewItemViewModel>();
    public IReadOnlyList<GoalProgressItemViewModel> Goals { get; set; } = Array.Empty<GoalProgressItemViewModel>();
    public IReadOnlyList<RecentWorkoutViewModel> RecentWorkouts { get; set; } = Array.Empty<RecentWorkoutViewModel>();
    public IReadOnlyList<BarChartItemViewModel> WeeklyWorkouts { get; set; } = Array.Empty<BarChartItemViewModel>();
}

public class PlanOverviewItemViewModel
{
    public int PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int Duration { get; set; }
    public int DayCount { get; set; }
    public int ExerciseCount { get; set; }
}

public class GoalProgressItemViewModel
{
    public int GoalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string GoalType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public float CurrentValue { get; set; }
    public float TargetValue { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int Percent { get; set; }
}

public class RecentWorkoutViewModel
{
    public int LogId { get; set; }
    public DateTime WorkoutDate { get; set; }
    public int DurationMinutes { get; set; }
    public string EnergyLevel { get; set; } = string.Empty;
    public int DetailCount { get; set; }
    public string Note { get; set; } = string.Empty;
}

public class BarChartItemViewModel
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
    public int MaxValue { get; set; }
    public string Hint { get; set; } = string.Empty;
}
