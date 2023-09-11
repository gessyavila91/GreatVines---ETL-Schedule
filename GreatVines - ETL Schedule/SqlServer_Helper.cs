namespace SftpExample; 

using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public class SqlServer_Helper {
    private readonly string _connectionString;

    public SqlServer_Helper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void test() {
        string connectionString = "Data Source=tu_servidor;Initial Catalog=tu_base_de_datos;User ID=tu_usuario;Password=tu_contraseña";

            // Ejemplo de cómo llamar a un Stored Procedure sin parámetros:
            DataTable result = ExecuteStoredProcedure("NombreDelStoredProcedure");

            // Si tu Stored Procedure requiere parámetros, puedes hacerlo así:
            SqlParameter[] parameters = 
            {
                new SqlParameter("@param1", SqlDbType.Int) { Value = 123 },
                new SqlParameter("@param2", SqlDbType.VarChar) { Value = "test" }
            };
            DataTable resultWithParams = ExecuteStoredProcedure("NombreDelStoredProcedureConParametros", parameters);
            
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
                csv.Append(row[i].ToString().Replace(",", ";"));// Reemplazamos comas en el dato para evitar conflictos
                csv.Append(i == table.Columns.Count - 1 ? "\n" : ",");
            }
        }

        return csv.ToString();
    }
}
