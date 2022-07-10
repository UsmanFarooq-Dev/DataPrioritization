using System.Data;
using Dapper;
using System.Data.SQLite;
using DataPrioritization.Models;

namespace DataPrioritization.DataAccess;

public static class Db
{

    public static List<Employee> GetEmployees(string connectionString)
    {
        using (IDbConnection con = new SQLiteConnection(connectionString))
        {
            //var employees = con.Query<Employee>("Select Id, Name EmpName, Age from Employees;");
            var employees = con.Query<Employee>("Select * from Employees;").AsList();
            return employees;
        }

    }

    public static int AddEmployee(string connectionString, Employee employee)
    {
        using (IDbConnection con = new SQLiteConnection(connectionString))
        {
            string qry = $"Insert into Employees (Name, Age) Values('{employee.Name}', {employee.Age})";

            var result = con.Execute(qry);
            return result;
        }

    }

    //private static string GetConnectionString()
    //{ 
    //    return ConfigurationManager
    //}



}
