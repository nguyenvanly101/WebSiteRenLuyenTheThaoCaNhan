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

        public int Sets { get; set; }
        public int Reps { get; set; }
        public int RestTime { get; set; }

        // Navigation
        [ForeignKey("DayID")]
        public WorkoutDay WorkoutDay { get; set; }

        [ForeignKey("ExerciseID")]
        public Exercise Exercise { get; set; }
    }
}
