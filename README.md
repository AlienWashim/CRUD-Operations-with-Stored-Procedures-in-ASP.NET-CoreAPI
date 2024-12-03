# CRUD Operations with Stored Procedures in ASP.NE Core API

## What is a Stored Procedure?
A stored procedure is a set of SQL queries that are stored in a database and can be executed as a single unit. Unlike direct SQL queries which are sent to the database on demand, stored procedures are precompiled and stored in the database for reuse, making them more efficient, secure, and manageable for complex operations.

## Why Use Stored Procedures?
* **Performance**: Stored procedures are precompiled and optimized by the database engine, making them faster than running ad-hoc queries repeatedly.
* **Reusability**: You can call a stored procedure multiple times with different parameters without rewriting the same SQL logic.
* **Security**: Stored procedures can help encapsulate logic, ensuring that users cannot directly manipulate the underlying tables. It also allows for better permission management.
* **Data Integrity**: Stored procedures can help enforce business rules and maintain data integrity by performing multiple operations within a single transaction.
* **Maintainability**: With stored procedures, your SQL code is centralized in the database, reducing duplication and making updates easier.


## Direct SQL vs. Stored Procedures
**Direct SQL Queries**:
* Best for simple, one-off queries.
* More flexible for dynamic queries.
* Easier to implement for quick prototyping.
* Suitable for low-complexity operations.

**Stored Procedures**:
* Ideal for complex, reusable logic and multiple operations.
* Optimized for performance (precompiled).
* Enforces security and data integrity.
* Manages transactions efficiently.
* Ensures consistent data access across the application.


## When to Use Each?
* Use Direct SQL for simple, dynamic queries where flexibility is key.
* Use Stored Procedures for reusable, secure, and performance-critical operations that need to ensure data integrity and consistency.

![Pagination Example](https://github.com/AlienWashim/CRUD-Operations-with-Stored-Procedures-in-ASP.NET-CoreAPI/blob/04d15e5851ee8d267713c782de510d647a22b3de/StoredProcedure.png)

## Stored Procedure Code Example (SQL)
```sql
-- Stored Procedure for GetAllPersons
CREATE PROCEDURE GetAllPersons
AS
BEGIN
    SELECT Id, Name, Salary FROM Persons
END

-- Stored Procedure for GetPersonById
CREATE PROCEDURE GetPersonById
    @Id INT
AS
BEGIN
    SELECT Id, Name, Salary FROM Persons WHERE Id = @Id
END

-- Stored Procedure for AddPerson
CREATE PROCEDURE AddPerson
    @Name NVARCHAR(100),
    @Salary DECIMAL
AS
BEGIN
    INSERT INTO Persons (Name, Salary)
    VALUES (@Name, @Salary)
END

-- Stored Procedure for UpdatePerson
CREATE PROCEDURE UpdatePerson
    @Id INT,
    @Name NVARCHAR(100),
    @Salary DECIMAL
AS
BEGIN
    UPDATE Persons
    SET Name = @Name, Salary = @Salary
    WHERE Id = @Id
END

-- Stored Procedure for DeletePerson
CREATE PROCEDURE DeletePerson
    @Id INT
AS
BEGIN
    DELETE FROM Persons WHERE Id = @Id
END
```

## Example in C# Controller

```csharp
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly DatabaseServices _databaseServices;

        public PersonController(DatabaseServices databaseServices)
        {
            _databaseServices = databaseServices;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Person>> GetPersons()
        {
            var persons = _databaseServices.GetPersons();
            return persons;
        }

        [HttpGet("{id}")]
        public ActionResult<Person> GetPersonById(int id)
        {
            var person = _databaseServices.GetPersonById(id);
            if (person == null)
            {
                return NotFound();
            }
            return Ok(person);
        }

        [HttpPost]
        public ActionResult PostPerson([FromBody] Person person)
        {
            _databaseServices.AddPerson(person);

            return CreatedAtAction(nameof(GetPersons), new { Id = person.Id }, person);
        }

        [HttpPut("{id}")]
        public ActionResult PutPerson(int id, [FromBody] Person person)
        {
            var oldPerson = _databaseServices.GetPersonById(id);
            if (oldPerson == null)
            {
                return NotFound();
            }

            person.Id = id;

            _databaseServices.UpdatePerson(person);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeletePerson(int id)
        {
            Person oldPerson = _databaseServices.GetPersonById(id);
            if(oldPerson == null)
            {
                return NotFound();
            }

            _databaseServices.DeletePerson(id);
            return NoContent();
        }
    }
}
```

## Example in C# DatabaseServices
```csharp
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
```
## Model class
```csharp
namespace WebAPI.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
    }
}
```


## Conclusion
This repository demonstrates the use of stored procedures in an ASP.NET Core Web API to handle CRUD operations. By using stored procedures, you can enhance performance, ensure data security, and make your application more maintainable.
