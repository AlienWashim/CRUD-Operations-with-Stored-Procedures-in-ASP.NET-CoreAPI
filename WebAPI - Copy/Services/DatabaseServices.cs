using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebAPI.Models;

namespace WebAPI.Services
{
    public class DatabaseServices
    {
        private readonly string _connectionString;
        public DatabaseServices(string connectionString)
        {
            _connectionString = connectionString;
        }


        public List<Person> GetPersons()
        {
            List<Person> persons = new List<Person>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("GetAllPersons", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            persons.Add(new Person
                                {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Salary = reader.GetDecimal(2),
                            });
                        }
                    }
                }
            }

            return persons;
        }

        public Person GetPersonById(int id)
        {
            Person person = null;

            using (var  sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand command = new SqlCommand("GetPersonById", sqlConnection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            person = new Person
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Salary = reader.GetDecimal(2)
                            };
                        }
                    }
                }
            }
            return person;
        }

        public void AddPerson(Person person)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("AddPerson", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", person.Id);
                    command.Parameters.AddWithValue("@Name", person.Name);
                    command.Parameters.AddWithValue("@Salary", person.Salary);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePerson(Person person)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("UpdatePerson", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", person.Id);
                    command.Parameters.AddWithValue("@Name", person.Name);
                    command.Parameters.AddWithValue("@Salary", person.Salary);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeletePerson(int id)
        {
            using (var  sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = new SqlCommand("DeletePerson", sqlConnection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
