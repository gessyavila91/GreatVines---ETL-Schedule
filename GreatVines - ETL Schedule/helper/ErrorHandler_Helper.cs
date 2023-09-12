namespace GreatVibesSchedule.helper;

using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

public class ErrorHandler_Helper {
    
    private static string logFilePath = @"log.txt";
    
    public static void HandleException(Exception ex, string className, string functionName) {
        LogErrorToFile(ex, className, functionName);
        throw ex;
    }

    private static void LogErrorToFile(Exception ex, string className, string functionName) {
        
        using StreamWriter writer = new StreamWriter(logFilePath, append: true);

        writer.WriteLine("---------------------------------------------------------");
        writer.WriteLine($"Date/Time: {DateTime.Now}");
        writer.WriteLine($"Class: {className}, Function: {functionName}");
        writer.WriteLine($"Error source: {ex.Source}");
        writer.WriteLine($"Error message: {ex.Message}");
        writer.WriteLine($"Stack trace: {ex.StackTrace}");
        writer.WriteLine("---------------------------------------------------------");
    }

    private static void LogErrorToDatabase(Exception ex, string className, string methodName) {
        string connectionString = "";// Tu cadena de conexión aquí

        string query = @"INSERT INTO ErrorLogs(Date, ClassName, MethodName, ErrorMessage, StackTrace)
                         VALUES (@Date, @ClassName, @MethodName, @ErrorMessage, @StackTrace)";

        using SqlConnection connection = SqlServer_Helper.GetDbConnection();
        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@Date", DateTime.Now);
        command.Parameters.AddWithValue("@ClassName", className);
        command.Parameters.AddWithValue("@MethodName", methodName);
        command.Parameters.AddWithValue("@ErrorMessage", ex.Message);
        command.Parameters.AddWithValue("@StackTrace", ex.StackTrace);

        connection.Open();

        command.ExecuteNonQuery();
    }
}
