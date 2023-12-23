using Dapper;
using WebApiWithDapper.Models;
using WebApiWithDapper.Services;

namespace WebApiWithDapper.Endpoints
{
    public static class EmployeeEndpoints
    {
        public static void MapEmployeeEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("employees", async (SqlConnectionFactory sqlConnectionFactory) =>
            {
                using var connection = sqlConnectionFactory.Create();

                const string sql = "SELECT Id, Name, Email, Phone, Salary, Department From Employees";

                var employees = await connection.QueryAsync<Employee>(sql);

                return Results.Ok(employees);
            });

            builder.MapGet("employees/{id}", async (Guid id, SqlConnectionFactory sqlConnectionFactory) =>
            {
                using var connection = sqlConnectionFactory.Create(); 
                
                const string sql = """
                                   SELECT Id, Name, Email, Phone, Salary, Department 
                                   From Employees 
                                   WHERE Id = @CustomerId
                                   """;

                var employee = await connection.QuerySingleOrDefaultAsync<Employee>(sql, new {CustomerId = id});

                return employee is not null ? Results.Ok(employee) : Results.NotFound();
            });

            builder.MapPost("employees", async (Employee employee, SqlConnectionFactory sqlConnectionFactory) =>
            {
                using var connection = sqlConnectionFactory.Create();

                const string sql = """
                INSERT INTO Employees (Id, Name, Email, Phone, Salary, Department)
                     VALUES (@Id, @Name, @Email, @Phone, @Salary, @Department)
                
                """;

                Guid id = employee.Id = Guid.NewGuid();
                await connection.ExecuteAsync(sql, employee);

                return Results.Ok(id);
            });

            builder.MapPut("employees/{id}", async (Employee employee, SqlConnectionFactory sqlConnectionFactory) =>
            {
                using var connection = sqlConnectionFactory.Create();

                const string sql = """
                        UPDATE Employees SET Name = @Name, Email = @Email, 
                        Phone = @Phone, Salary = @Salary, 
                        Department = @Department WHERE Id = @Id     
                """;

                await connection.ExecuteAsync(sql, employee);

                return Results.Ok(employee);
            });

            builder.MapDelete("employees/{id}", async (Guid id, SqlConnectionFactory sqlConnectionFactory) =>
            {
                using var connection = sqlConnectionFactory.Create();

                const string sql = "DELETE FROM Employees WHERE Id = @EmployeeId";

                await connection.ExecuteAsync(sql, new {EmployeeId = id});

                return Results.Ok();
            });
        }
    }
}
