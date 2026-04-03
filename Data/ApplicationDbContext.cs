using Microsoft.EntityFrameworkCore;
using WebsiteRenLuyenTheThaoCaNhan.Models;
namespace WebsiteRenLuyenTheThaoCaNhan.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<WorkoutDay> WorkoutDays { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<WorkoutLog> WorkoutLogs { get; set; }
        public DbSet<WorkoutLogDetail> WorkoutLogDetails { get; set; }
        public DbSet<Goal> Goals { get; set; }
    }
}
