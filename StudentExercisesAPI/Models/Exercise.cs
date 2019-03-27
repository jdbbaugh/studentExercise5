using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentExercisesAPI.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Language { get; set; }
        public List<string> AssignedStudents { get; set; } = new List<string>();
    }
}
