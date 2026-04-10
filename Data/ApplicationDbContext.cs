using Microsoft.EntityFrameworkCore;
using WebsiteRenLuyenTheThaoCaNhan.Models;
namespace WebsiteRenLuyenTheThaoCaNhan.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<WorkoutPlan> WorkoutPlans => Set<WorkoutPlan>();
        public DbSet<WorkoutDay> WorkoutDays => Set<WorkoutDay>();
        public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
        public DbSet<WorkoutLog> WorkoutLogs => Set<WorkoutLog>();
        public DbSet<WorkoutLogDetail> WorkoutLogDetails => Set<WorkoutLogDetail>();
        public DbSet<Goal> Goals => Set<Goal>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(user => user.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(user => user.Email)
                .IsUnique();

            modelBuilder.Entity<WorkoutDay>()
                .HasIndex(day => new { day.PlanID, day.DayNumber })
                .IsUnique();

            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(item => item.Exercise)
                .WithMany(exercise => exercise.WorkoutExercises)
                .HasForeignKey(item => item.ExerciseID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkoutLogDetail>()
                .HasOne(item => item.Exercise)
                .WithMany(exercise => exercise.WorkoutLogDetails)
                .HasForeignKey(item => item.ExerciseID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
