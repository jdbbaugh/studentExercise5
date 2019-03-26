using System;
using System.Collections.Generic;
using System.Text;

namespace StudentExercisesAPI.Models
{
    public class Cohort
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> StudentList { get; set; } = new List<string>();

        public List<string> InstructorList { get; set; } = new List<string>();

        public List<Student> Students { get; set; } = new List<Student>();
        public List<Instructor> Instructors { get; set; } = new List<Instructor>();
    }
}
