namespace WebsiteRenLuyenTheThaoCaNhan.Models
{
    public class Exercise
    {
        public int ExerciseID { get; set; }
        public string Name { get; set; }
        public string MuscleGroup { get; set; }
        public string Equipment { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }

        public List<WorkoutExercise> WorkoutExercises { get; set; } // Navigation property to workout exercises that include this exercise
        public List<WorkoutLogDetail> WorkoutLogDetails { get; set; } // Navigation property to workout log details that include this exercise
    }
}
