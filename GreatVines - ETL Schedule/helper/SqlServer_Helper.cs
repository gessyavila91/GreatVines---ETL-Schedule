namespace SftpExample; 

using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public class SqlServer_Helper {
    public string _connectionString;

    string query = "SELECT * FROM OITM T0 WHERE  T0.ValidFor='Y';";

    public SqlServer_Helper(string connectionString)
    {
        _connectionString = connectionString;
    }
    public SqlServer_Helper(string host, string database, string userId, string password)
    {
        _connectionString = BuildConnectionString(host, database, userId, password);
    }
    private string BuildConnectionString(string host, string database, string userId, string password)
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
        {
            DataSource = host,
            InitialCatalog = database,
            UserID = userId,
            Password = password
        };
        return builder.ToString();
    }

    public void test() {
        
        DataTable result = ExecuteStoredProcedure("Products");

        Console.WriteLine(ConvertToCsv(result));

    }

    public DataTable ExecuteStoredProcedure(string storedProcedureName, SqlParameter[] parameters = null)
    {
        DataTable result = new DataTable();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(storedProcedureName, connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(result);
                }
            }
        }

        return result;
    }

    public void ExecuteQuery(string query) {
        using (SqlConnection connection = new SqlConnection(_connectionString)) {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection)) {
                // Supongamos que es un query de SELECT y queremos leer los resultados
                using (SqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        // Aquí procesas cada fila. Por simplicidad, solo vamos a imprimir la primera columna
                        Console.WriteLine(reader[0].ToString());
                    }
                }
            }
        }
    }

    public string ConvertToCsv(DataTable table) {
        StringBuilder csv = new StringBuilder();

        // Escribir los nombres de las columnas
        for (int i = 0; i < table.Columns.Count; i++) {
            csv.Append(table.Columns[i].ColumnName);
            csv.Append(i == table.Columns.Count - 1 ? "\n" : ",");
        }

        // Escribir las filas
        foreach (DataRow row in table.Rows) {
            for (int i = 0; i < table.Columns.Count; i++) {
                // Si el tipo de dato es cadena, rodeamos con comillas
                if (table.Columns[i].DataType == typeof(string)) {
                    csv.Append("\"");
                    csv.Append(row[i].ToString().Replace("\"", "\"\""));// Escapamos comillas dobles si ya existen
                    csv.Append("\"");
                }
                else {
                    csv.Append(row[i].ToString()
                        .Replace(",", ";"));// Reemplazamos comas en el dato para evitar conflictos
                }

                csv.Append(i == table.Columns.Count - 1 ? "\n" : ",");
            }
        }

        return csv.ToString();
    }

}
