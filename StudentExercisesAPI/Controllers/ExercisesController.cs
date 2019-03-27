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
    public class ExercisesController : ControllerBase
    {
        public SqlConnection Connection
        {
            get
            {
                string connectionSTring = "Server=localhost\\SQLExpress;Database=StudentExercises2;Integrated Security=true";
                return new SqlConnection(connectionSTring);
            }
        }

        //GET: api/Exercises
        [HttpGet]
        public IEnumerable<Exercise> Get(string include,string exerciseName = "" ,string exerciseLanguage= "")
        {
            string queryName = (exerciseName == "") ? "%" : exerciseName;
            string queryLanguage = (exerciseLanguage == "") ? "%" : exerciseLanguage;
            if (include != "students")
            {
            

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"SELECT id, ExerciseName, ExerciseLanguage FROM Exercise WHERE (ExerciseName LIKE '{queryName}' AND ExerciseLanguage LIKE '{queryLanguage}');";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Exercise> exercises = new List<Exercise>();
                    while (reader.Read())
                    {
                        Exercise exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            Language = reader.GetString(reader.GetOrdinal("ExerciseLanguage")),
                        };
                        exercises.Add(exercise);
                    }

                    reader.Close();
                    return exercises;
                }
            }
        }
            else
            {
                List<Exercise> exercises = new List<Exercise>();
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT id, ExerciseName, ExerciseLanguage FROM Exercise;";

                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            Exercise exercise = new Exercise
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                Language = reader.GetString(reader.GetOrdinal("ExerciseLanguage"))
                            };
                            exercises.Add(exercise);
                        }
                        reader.Close();
                    }
                }

                using (SqlConnection conn2 = Connection)
                {
                    conn2.Open();
                    using (SqlCommand cmd = conn2.CreateCommand())
                    {
                        foreach (Exercise exercise in exercises)
                        {
                            cmd.CommandText = "SELECT e.ExerciseName, e.ExerciseLanguage, s.FirstName, s.LastName " +
                                              "FROM AssignedExercise a " +
                                              "JOIN Exercise e ON a.ExerciseId = e.id " +
                                              "JOIN Student s ON a.StudentId = s.id " +
                                              $"WHERE e.id = {exercise.Id}";
                            SqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                string studentName = $"{reader.GetString(reader.GetOrdinal("FirstName"))} {reader.GetString(reader.GetOrdinal("LastName"))}";
                                exercise.AssignedStudents.Add(studentName);
                            }
                            reader.Close();
                        }

                        return exercises;
                    }
                }
            }
    }

        //GET apki/Exercise/5
        [HttpGet("{id}", Name = "GetExercise")]
        public Exercise Get(int id, string include)
        {
          Exercise exercise = null;
            if (include != "students")
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT id, ExerciseName, ExerciseLanguage 
                                        FROM Exercise 
                                        WHERE id = @id;";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            exercise = new Exercise
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                Language = reader.GetString(reader.GetOrdinal("ExerciseLanguage")),
                            };
                        }

                        reader.Close();
                        return exercise;
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
                        cmd.CommandText = @"SELECT id, ExerciseName, ExerciseLanguage 
                                        FROM Exercise 
                                        WHERE id = @id;";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();
                        
                        if (reader.Read())
                        {
                            exercise = new Exercise
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                Language = reader.GetString(reader.GetOrdinal("ExerciseLanguage")),
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
                        cmd.CommandText = "SELECT s.FirstName, s.LastName FROM AssignedExercise a " +
                                          "JOIN Exercise e ON a.ExerciseId = e.id " +
                                          "JOIN Student s ON a.StudentId = s.id " +
                                          "WHERE e.id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", exercise.Id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            string studentName =
                                $"{reader.GetString(reader.GetOrdinal("FirstName"))} {reader.GetString(reader.GetOrdinal("LastName"))}";
                            exercise.AssignedStudents.Add(studentName);
                        }
                        reader.Close();
                        return exercise;
                    }
                }
            }
        }

        //POST: api/Instructors
        [HttpPost]
        public ActionResult Post([FromBody] Exercise newExercise)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Exercise (ExerciseName, ExerciseLanguage)
                                            OUTPUT INSERTED.Id 
                                            VALUES (@exerciseName ,@exerciseLanguage)";
                    cmd.Parameters.Add(new SqlParameter("@exerciseName", newExercise.Name));
                    cmd.Parameters.Add(new SqlParameter("@exerciseLanguage", newExercise.Language));

                    int newId = (int)cmd.ExecuteScalar();
                    newExercise.Id = newId;
                    return CreatedAtRoute("GetExercise", new {id = newId}, newExercise);
                }
            }
        }

        //PUT: api?Exercises/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Exercise exercise)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Exercise
                                        SET ExerciseName = @exerciseName,
                                            ExerciseLanguage = @exerciseLanguage
                                        WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@exerciseName", exercise.Name));
                    cmd.Parameters.Add(new SqlParameter("@exerciseLanguage", exercise.Language));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();

                }
            }
        }

        //DELETE: api/Exercises/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Exercise WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}