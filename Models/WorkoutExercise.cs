using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteRenLuyenTheThaoCaNhan.Models
{
    public class WorkoutExercise
    {
        [Key]
        public int ID { get; set; }

        // Foreign Keys
        public int DayID { get; set; }
        public int ExerciseID { get; set; }

        [Range(1, 20)]
        public int Sets { get; set; }

        [Range(1, 50)]
        public int Reps { get; set; }

        [Range(0, 600)]
        public int RestTime { get; set; }

        public int DisplayOrder { get; set; } = 1;

        // Navigation
        [ForeignKey("DayID")]
        public WorkoutDay WorkoutDay { get; set; } = null!;

        [ForeignKey("ExerciseID")]
        public Exercise Exercise { get; set; } = null!;
    }
}
