using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentExercisesAPI.Models;

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        public SqlConnection Connection
        {
            get
            {
                string connectionSTring = "Server=localhost\\SQLExpress;Database=StudentExercises2;Integrated Security=true";
                return new SqlConnection(connectionSTring);
            }
        }


        // GET: api/Students
        [HttpGet]
        public IEnumerable<Student> Get(string firstName = "", string lastName = "", string slackHandle = "", string include = "")
        {
            string queryFn = (firstName == "") ? "%" : firstName;
            string queryLn = (lastName == "") ? "%" : lastName;
            string querySh = (slackHandle == "") ? "%" : slackHandle;

            if (include != "exercises")
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"SELECT s.id, s.firstName, s.lastName,
                                               s.slackHandle, s.cohortId, c.cohortName
                                        FROM Student s INNER JOIN Cohort c On s.cohortId = c.id
                                        WHERE (s.firstName LIKE '{queryFn}' AND s.lastName LIKE '{queryLn}'
                                             AND s.slackHandle LIKE '{querySh}')";
                        SqlDataReader reader = cmd.ExecuteReader();

                        List<Student> students = new List<Student>();
                        while (reader.Read())
                        {
                            Student student = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("firstName")),
                                LastName = reader.GetString(reader.GetOrdinal("lastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("slackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("cohortId")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                            };
                            students.Add(student);
                        }

                        reader.Close();
                        return students;
                    }
                }
            }
            else
            {
                List<Student> students = new List<Student>();
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"SELECT s.id, s.firstName, s.lastName,
                                               s.slackHandle, s.cohortId, c.cohortName
                                        FROM Student s INNER JOIN Cohort c On s.cohortId = c.id
                                        WHERE (s.firstName LIKE '{queryFn}' AND s.lastName LIKE '{queryLn}'
                                             AND s.slackHandle LIKE '{querySh}')";
                        SqlDataReader reader = cmd.ExecuteReader();

                        
                        while (reader.Read())
                        {
                            Student student = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("firstName")),
                                LastName = reader.GetString(reader.GetOrdinal("lastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("slackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("cohortId")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                            };
                            students.Add(student);
                        }
                        reader.Close();
                    }
                }

                using (SqlConnection conn2 = Connection)
                {
                    conn2.Open();
                    using (SqlCommand cmd = conn2.CreateCommand())
                    {
                        foreach (Student student in students)
                        {
                            cmd.CommandText = $@"SELECT e.id, e.ExerciseName, e.ExerciseLanguage
                                                   FROM AssignedExercise ae 
                                                   JOIN Exercise e ON ae.ExerciseId = e.id
                                                  WHERE ae.StudentId = {student.Id}";
                            SqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                Exercise exercise = new Exercise
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                    Language = reader.GetString(reader.GetOrdinal("ExerciseLanguage"))
                                };
                                student.Exercises.Add(exercise);
                            }
                            reader.Close();
                        }

                        return students;
                    }
                }
            }
        }

        // GET: api/Students/5?include=exercise
        [HttpGet("{id}", Name = "GetStudent")]
        public Student Get(int id, string include)
        {
            Student student = null;

            if (include != "exercises")
            {
            

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.id, s.firstName, s.lastName,
                                               s.slackHandle, s.cohortId, c.cohortName
                                        FROM Student s INNER JOIN Cohort c ON s.cohortId = c.id
                                        WHERE s.id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                        
                    if (reader.Read())
                    {
                        student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FirstName = reader.GetString(reader.GetOrdinal("firstName")),
                            LastName = reader.GetString(reader.GetOrdinal("lastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("slackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("cohortId")),
                            CohortName = reader.GetString(reader.GetOrdinal("cohortname")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("cohortId")),
                                Name = reader.GetString(reader.GetOrdinal("cohortName"))
                            }
                        };
                    }

                    reader.Close();
                    return student;
                }
            }

        }
            else
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT s.id, s.firstName, s.lastName,
                                               s.slackHandle, s.cohortId, c.cohortName
                                        FROM Student s 
                                        INNER JOIN Cohort c ON s.cohortId = c.id
                                        WHERE s.id = @id;";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        
                        if (reader.Read())
                        {
                            student = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                FirstName = reader.GetString(reader.GetOrdinal("firstName")),
                                LastName = reader.GetString(reader.GetOrdinal("lastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("slackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("cohortId")),
                                CohortName = reader.GetString(reader.GetOrdinal("cohortname")),
                                Cohort = new Cohort
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("cohortId")),
                                    Name = reader.GetString(reader.GetOrdinal("cohortName"))
                                }
                            };
                        }

                        reader.Close();
                    }
                }

                using (SqlConnection conn2 = Connection)
                {
                    conn2.Open();
                    using (SqlCommand cmd = conn2.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT e.Id, e.ExerciseName, e.ExerciseLanguage " +
                                          "FROM AssignedExercise a " +
                                          "JOIN Exercise e ON a.ExerciseId = e.Id " +
                                          $"WHERE a.StudentId = {id}";
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            Exercise exercise = new Exercise
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                Language = reader.GetString(reader.GetOrdinal("ExerciseLanguage"))
                            };
                            student.Exercises.Add(exercise);
                        }
                        reader.Close();
                        return student;
                    }
                }
            }
    }

        // POST: api/Students
        [HttpPost]
        public ActionResult Post([FromBody] Student newStudent)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Student (firstName, lastName, slackHandle, cohortId)
                                               OUTPUT INSERTED.id
                                               VALUES (@firstName, @lastName, @slackHandle, @cohortId)";
                    cmd.Parameters.Add(new SqlParameter("@firstName", newStudent.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", newStudent.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", newStudent.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", newStudent.CohortId));

                    int newId = (int) cmd.ExecuteScalar();
                    newStudent.Id = newId;
                    return CreatedAtRoute("GetStudent", new {id = newId}, newStudent);

                }
            }
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Student student)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE student
                                            SET firstname = @firstname,
                                                lastname = @lastname,
                                                slackhandle = @slackhandle,
                                                cohortid = @cohortid
                                            WHERE id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@firstname", student.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastname", student.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackhandle", student.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@cohortid", student.CohortId));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM student WHERE id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
