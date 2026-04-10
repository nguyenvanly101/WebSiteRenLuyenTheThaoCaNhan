namespace WebsiteRenLuyenTheThaoCaNhan.ViewModels;

public class ProgressViewModel
{
    public string FullName { get; set; } = string.Empty;
    public int TotalSessions { get; set; }
    public int SessionsThisMonth { get; set; }
    public float TotalVolume { get; set; }
    public int GoalCompletionRate { get; set; }
    public IReadOnlyList<GoalProgressItemViewModel> GoalProgress { get; set; } = Array.Empty<GoalProgressItemViewModel>();
    public IReadOnlyList<BarChartItemViewModel> MonthlyWorkouts { get; set; } = Array.Empty<BarChartItemViewModel>();
    public IReadOnlyList<BarChartItemViewModel> MuscleGroups { get; set; } = Array.Empty<BarChartItemViewModel>();
    public IReadOnlyList<RecentWorkoutViewModel> RecentWorkouts { get; set; } = Array.Empty<RecentWorkoutViewModel>();
}
