using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebsiteRenLuyenTheThaoCaNhan.Infrastructure;
using WebsiteRenLuyenTheThaoCaNhan.Models;

namespace WebsiteRenLuyenTheThaoCaNhan.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.EnsureCreatedAsync();

            if (!await context.Users.AnyAsync())
            {
                var hasher = new PasswordHasher<User>();

                var admin = new User
                {
                    FullName = "System Admin",
                    Username = "admin",
                    Email = "admin@pulseforge.local",
                    Role = AppRoles.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                admin.PasswordHash = hasher.HashPassword(admin, "Admin@123456");

                var member = new User
                {
                    FullName = "Demo Member",
                    Username = "member",
                    Email = "member@pulseforge.local",
                    Role = AppRoles.User,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                member.PasswordHash = hasher.HashPassword(member, "Member@123456");

                context.Users.AddRange(admin, member);
            }

            if (!await context.Exercises.AnyAsync())
            {
                context.Exercises.AddRange(
                    new Exercise
                    {
                        Name = "Barbell Back Squat",
                        MuscleGroup = "Lower Body",
                        Equipment = "Barbell",
                        Difficulty = "Intermediate",
                        Description = "Compound strength exercise for quads, glutes and overall lower-body power.",
                        VideoUrl = "https://www.youtube.com/watch?v=ultWZbUMPL8",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Exercise
                    {
                        Name = "Romanian Deadlift",
                        MuscleGroup = "Posterior Chain",
                        Equipment = "Barbell",
                        Difficulty = "Intermediate",
                        Description = "Improves hamstrings, glutes and hip hinge mechanics.",
                        VideoUrl = "https://www.youtube.com/watch?v=2SHsk9AzdjA",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Exercise
                    {
                        Name = "Bench Press",
                        MuscleGroup = "Chest",
                        Equipment = "Barbell",
                        Difficulty = "Intermediate",
                        Description = "Classic push movement for chest, shoulders and triceps.",
                        VideoUrl = "https://www.youtube.com/watch?v=rT7DgCr-3pg",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Exercise
                    {
                        Name = "Pull-Up",
                        MuscleGroup = "Back",
                        Equipment = "Pull-up Bar",
                        Difficulty = "Advanced",
                        Description = "Bodyweight pulling exercise for lats, biceps and grip strength.",
                        VideoUrl = "https://www.youtube.com/watch?v=eGo4IYlbE5g",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Exercise
                    {
                        Name = "Dumbbell Shoulder Press",
                        MuscleGroup = "Shoulders",
                        Equipment = "Dumbbell",
                        Difficulty = "Beginner",
                        Description = "Stable overhead press to improve shoulder strength and posture.",
                        VideoUrl = "https://www.youtube.com/watch?v=qEwKCR5JCog",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Exercise
                    {
                        Name = "Walking Lunge",
                        MuscleGroup = "Lower Body",
                        Equipment = "Dumbbell",
                        Difficulty = "Beginner",
                        Description = "Single-leg exercise that builds balance and unilateral strength.",
                        VideoUrl = "https://www.youtube.com/watch?v=L8fvypPrzzs",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Exercise
                    {
                        Name = "Plank Hold",
                        MuscleGroup = "Core",
                        Equipment = "Bodyweight",
                        Difficulty = "Beginner",
                        Description = "Core stability drill for trunk endurance and anti-extension strength.",
                        VideoUrl = "https://www.youtube.com/watch?v=ASdvN_XEl_c",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Exercise
                    {
                        Name = "Seated Cable Row",
                        MuscleGroup = "Back",
                        Equipment = "Cable Machine",
                        Difficulty = "Beginner",
                        Description = "Horizontal pull for mid-back strength and scapular control.",
                        VideoUrl = "https://www.youtube.com/watch?v=UCXxvVItLoM",
                        CreatedAt = DateTime.UtcNow
                    });
            }

            await context.SaveChangesAsync();
        }
    }
}
