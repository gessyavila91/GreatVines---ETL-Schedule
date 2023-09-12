using System.Reflection;
namespace GreatVibesSchedule.helper;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
public static class SqlServer_Helper {
    private static string getConnectionString(String database = null) {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        try {
            builder.DataSource = "192.168.1.21";
            builder.InitialCatalog = database ?? "SBO_Tequilaspremium_Prod";
            builder.UserID = "Desarrollos";
            builder.Password = "D3v5ll0@7r@22$";
        } catch (Exception ex) {
            ErrorHandler_Helper.HandleException(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
        }
        
        return builder.ToString();
    }
    public static SqlConnection GetDbConnection(String database = null) {
        SqlConnection connection = null;

        try {
            connection = new SqlConnection(getConnectionString(database));
        } catch (Exception ex) {
            ErrorHandler_Helper.HandleException(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
        }

        return connection;
    }

    public static DataTable ExecuteStoredProcedure(string storedProcedureName, SqlParameter[] parameters = null) {
        string connectionString = getConnectionString();
        DataTable result = new DataTable();

        try {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, connection)) {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null) {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd)) {
                        adapter.Fill(result);
                    }
                }
            }
        } catch (Exception ex) {
            ErrorHandler_Helper.HandleException(ex, MethodBase.GetCurrentMethod().DeclaringType.Name,
                MethodBase.GetCurrentMethod().Name);
        }

        return result;
    }

    public static DataTable ExecuteQuery(string query) {
        using SqlConnection connection = GetDbConnection();
        using SqlCommand command = new SqlCommand(query, connection);
        DataTable result = new DataTable();

        try {
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            result.Load(reader);
            connection.Close();
        } catch (Exception ex) {
            ErrorHandler_Helper.HandleException(ex, MethodBase.GetCurrentMethod().DeclaringType.Name,
                MethodBase.GetCurrentMethod().Name);
        }

        return result;
    }

    public static void InsertIntoDatabase(string tableName, params SqlParameter[] parameters) {
        StringBuilder query = new StringBuilder($"INSERT INTO {tableName} (");
        for (int i = 0; i < parameters.Length; i++) {
            query.Append(parameters[i].ParameterName.TrimStart('@'));
            if (i < parameters.Length - 1)
                query.Append(", ");
        }
        query.Append(") VALUES (");
        for (int i = 0; i < parameters.Length; i++) {
            query.Append(parameters[i].ParameterName);
            if (i < parameters.Length - 1)
                query.Append(", ");
        }
        query.Append(")");
        
        using (SqlConnection connection = new SqlConnection(getConnectionString())) {
            connection.Open();
            
            using (SqlCommand command = new SqlCommand(query.ToString(), connection)) {
                command.Parameters.AddRange(parameters);
                
                command.ExecuteNonQuery();
                Console.WriteLine("El registro se ha insertado de manera exitosa en la base de datos.");
            }
        }
    }

}
