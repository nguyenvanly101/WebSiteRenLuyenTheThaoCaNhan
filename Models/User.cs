namespace WebsiteRenLuyenTheThaoCaNhan.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // e.g., "Admin", "User"

        public List<WorkoutPlan> WorkoutPlans { get; set; } // Navigation property to user's workout plans
        public List<Goal> Goals { get; set; } // Navigation property to user's goals
        public List<WorkoutLog> WorkoutLogs { get; set; }
    }
}
