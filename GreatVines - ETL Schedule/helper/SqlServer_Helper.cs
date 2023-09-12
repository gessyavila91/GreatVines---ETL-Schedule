namespace GreatVibesSchedule.helper;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
public static class SqlServer_Helper {
    private static string BuildConnectionString() {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder {
            DataSource = "192.168.1.21",
            InitialCatalog = "SBO_Tequilaspremium_Prod",
            UserID = "Desarrollos",
            Password = "D3v5ll0@7r@22$"
        };
        return builder.ToString();
    }

    public static DataTable ExecuteStoredProcedure(string storedProcedureName, SqlParameter[] parameters = null) {
        string connectionString = BuildConnectionString();
        DataTable result = new DataTable();

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

        return result;
    }

    public static DataTable ExecuteQuery(string query) {
        using SqlConnection connection = new SqlConnection(BuildConnectionString());
        using SqlCommand command = new SqlCommand(query, connection);

        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();

        var result = new DataTable();
        result.Load(reader);

        return result;
    }

}
