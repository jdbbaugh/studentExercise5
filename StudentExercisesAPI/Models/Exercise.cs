using System.Collections.Generic;

namespace StudentExercisesAPI.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public List<string> AssignedStudents { get; set; } = new List<string>();
    }
}
