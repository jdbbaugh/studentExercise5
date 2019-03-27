using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StudentExercisesAPI.Models
{
    public class Cohort
    {
        public int Id { get; set; }
        [Required]
        [StringLength(11, MinimumLength = 5)]
        [RegularExpression(@"(\bday\b|\bDay\b|\bevening\b|\bEvening\b)\s(\b\d{1,2})")]
        public string Name { get; set; }
        public List<string> StudentList { get; set; } = new List<string>();

        public List<string> InstructorList { get; set; } = new List<string>();

        public List<Student> Students { get; set; } = new List<Student>();
        public List<Instructor> Instructors { get; set; } = new List<Instructor>();
    }
}
