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

## Stored Procedure Code Example (SQL)
```sql
-- Stored Procedure to Get Paginated Persons
CREATE PROCEDURE GetPaginatedPersons
    @PageNumber INT,
    @PageSize INT
AS
BEGIN
    SELECT * FROM Persons
    ORDER BY Id
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
```

## Example in C# Controller

```csharp
// Controller method to fetch paginated persons
[HttpGet]
public ActionResult<IEnumerable<Person>> GetPaginatedPersons(int pageNumber = 1, int pageSize = 10)
{
    var persons = _databaseServices.GetPaginatedPersons(pageNumber, pageSize);
    return Ok(persons);
}
```

## Example in C# DatabaseServices
```csharp
// Database service method to call the stored procedure and get paginated persons
public List<Person> GetPaginatedPersons(int pageNumber, int pageSize)
{
    List<Person> persons = new List<Person>();

    using (var connection = new SqlConnection(_connectionString))
    {
        connection.Open();
        using (var command = new SqlCommand("GetPaginatedPersons", connection))
        {
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PageNumber", pageNumber);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    persons.Add(new Person
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Salary = reader.GetDecimal(2)
                    });
                }
            }
        }
    }

    return persons;
}
```

## Conclusion
This repository demonstrates the use of stored procedures in an ASP.NET Core Web API to handle CRUD operations. By using stored procedures, you can enhance performance, ensure data security, and make your application more maintainable.
