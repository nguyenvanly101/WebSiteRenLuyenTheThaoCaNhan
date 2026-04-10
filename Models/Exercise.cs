namespace WebsiteRenLuyenTheThaoCaNhan.Models
{
    public class Exercise
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int ExerciseID { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        public string MuscleGroup { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        public string Equipment { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(40)]
        public string Difficulty { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.StringLength(255)]
        public string VideoUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
        public ICollection<WorkoutLogDetail> WorkoutLogDetails { get; set; } = new List<WorkoutLogDetail>();
    }
}
