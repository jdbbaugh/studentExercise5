using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudentExercisesAPI.Models;

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CohortsController : Controller
    {
        public SqlConnection Connection
        {
            get
            {
                string connectionSTring = "Server=localhost\\SQLExpress;Database=StudentExercises2;Integrated Security=true";
                return new SqlConnection(connectionSTring);
            }
        }

        //GET: api/Cohorts
        [HttpGet]
        public IEnumerable<Cohort> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT c.id, c.cohortName, i.FirstName AS[Instructor-First], i.id AS InstructorId,i.LastName AS[Instructor-Last], i.SlackHandle AS [Instructor-Slack], s.id AS StudentId, s.FirstName AS[Student-First], s.LastName AS[Student-Last], s.SlackHandle AS [Student-Slack]" +
                                      "FROM Cohort c LEFT JOIN Instructor i ON i.CohortId = c.id LEFT JOIN Student s ON s.CohortId = c.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, Cohort> cohorts = new Dictionary<int, Cohort>();
                    Dictionary<int, Instructor> instructorSort = new Dictionary<int, Instructor>();
                    Dictionary<int, Student> studentSort = new Dictionary<int, Student>();
                    while (reader.Read())
                    {
                        int cohortId = reader.GetInt32(reader.GetOrdinal("id"));
                        if (!cohorts.ContainsKey(cohortId))
                        {
                            Cohort newCohort = new Cohort
                            {
                                Id = cohortId,
                                Name = reader.GetString(reader.GetOrdinal("CohortName"))
                            };

                        cohorts.Add(cohortId,newCohort);
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal(("StudentId"))))
                        {
                            int studentId = reader.GetInt32(reader.GetOrdinal("StudentId"));
                            if (!studentSort.ContainsKey(studentId))
                            {
                                Student newStudent = new Student
                                {
                                    Id = studentId,
                                    FirstName = reader.GetString(reader.GetOrdinal("Student-First")),
                                    LastName = reader.GetString(reader.GetOrdinal("Student-Last")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("Student-Slack")),
                                    CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("id"))
                                };
                                studentSort.Add(studentId, newStudent);

                                Cohort currentCohort = cohorts[cohortId];
                                currentCohort.Students.Add
                                (
                                    new Student
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                        FirstName = reader.GetString(reader.GetOrdinal("Student-First")),
                                        LastName = reader.GetString(reader.GetOrdinal("Student-Last")),
                                        SlackHandle = reader.GetString(reader.GetOrdinal("Student-Slack")),
                                        CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                                        CohortId = reader.GetInt32(reader.GetOrdinal("id"))
                                    }
                                );
                                string stuFirstName = reader.GetString(reader.GetOrdinal("Student-First"));
                                string stuLastName = reader.GetString(reader.GetOrdinal("Student-Last"));
                                string stuFullName = $"{stuFirstName} {stuLastName}";

                                Cohort currentCohort2 = cohorts[cohortId];
                                currentCohort2.StudentList.Add(stuFullName);


                            }
                        }
                            
                            if (!reader.IsDBNull(reader.GetOrdinal(("InstructorId"))))
                            {
                                int instructorId = reader.GetInt32(reader.GetOrdinal("InstructorId"));
                                if (!instructorSort.ContainsKey(instructorId))
                                {
                                    Instructor newInstructor = new Instructor
                                    {
                                        Id = instructorId,
                                        FirstName = reader.GetString(reader.GetOrdinal("Instructor-First")),
                                        LastName = reader.GetString(reader.GetOrdinal("Instructor-Last")),
                                        SlackHandle = reader.GetString(reader.GetOrdinal("Instructor-Slack")),
                                        CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                                        CohortId = reader.GetInt32(reader.GetOrdinal("id"))
                                    };
                                    instructorSort.Add(instructorId,newInstructor);

                                    Cohort currentCohort = cohorts[cohortId];
                                    currentCohort.Instructors.Add(
                                        new Instructor
                                        {
                                            Id = instructorId,
                                            FirstName = reader.GetString(reader.GetOrdinal("Instructor-First")),
                                            LastName = reader.GetString(reader.GetOrdinal("Instructor-Last")),
                                            SlackHandle = reader.GetString(reader.GetOrdinal("Instructor-Slack")),
                                            CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                                            CohortId = reader.GetInt32(reader.GetOrdinal("id"))
                                        }
                                    );
                                    string insFirstName = reader.GetString(reader.GetOrdinal("Instructor-First"));
                                    string insLastName = reader.GetString(reader.GetOrdinal("Instructor-Last"));
                                    string stuFullName = $"{insFirstName} {insLastName}";
                                    currentCohort.InstructorList.Add(stuFullName);
                            }
                                
                        }
                    }
                    reader.Close();
                    return cohorts.Values.ToList();
                }
            }
        }




        //GET: api/cohorts/5
        [HttpGet("{id}", Name = "GetCohort")]
        public Cohort Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, CohortName FROM Cohort WHERE id=@id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort cohort = null;
                    if (reader.Read())
                    {
                        cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("CohortName"))
                        };
                    }
                    reader.Close();
                    return cohort;
                }
            }
        }

        //POST: api/Cohorts
        [HttpPost]
        public ActionResult Post([FromBody] Cohort newCohort)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Cohort (CohortName)
                                            OUTPUT INSERTED.Id
                                            VALUES (@cohortName)";
                    cmd.Parameters.Add(new SqlParameter("@cohortName", newCohort.Name));

                    int newId = (int) cmd.ExecuteScalar();
                    newCohort.Id = newId;
                    return CreatedAtRoute("GetCohort", new {id = newId}, newCohort);
                }
            }
        }
        //PUT: api/cohorts/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Cohort cohort)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Cohort
                                            SET CohortName = @cohortName
                                        WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@cohortName", cohort.Name));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //DELETE: api/cohorts/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Cohort WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}