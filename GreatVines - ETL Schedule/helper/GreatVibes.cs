namespace GreatVibesSchedule.helper;

using System.Text;
using System.Data;

public static class GreatVibes {
    
    private static readonly string dateFormat = "yyyyMMdd_HHmmss";
    private static readonly string StoreProcedurePrefix = "SAP_GreatVines_";
    private static List<string> storedProceduresList = new() { "SAP_GreatVines_Products", "SAP_GreatVines_Distributors"};
    

    private static string clientName = "ClaseAzul";

    static GreatVibes() {

    }
    
    public static string GetFileName(DateTime now, string file = null) {
        return file == null ? $"{clientName}_{now.ToString(dateFormat)}.csv" :
            $"{clientName}_{file}_{now.ToString(dateFormat)}.csv";
    }

    public static void CreateFiles() {
        DateTime now = DateTime.Now;
        foreach (string storedProcedures in storedProceduresList) {
            CreateFiles(ConvertToCsv(SqlServer_Helper.ExecuteStoredProcedure(storedProcedures)),GetFileName(now,storedProcedures));
        }
    }
    public static void CreateFiles(string csv, string fileName, string filePath = ".") {
        try {
            string fullPath = Path.Combine(filePath, fileName);
            File.WriteAllText(fullPath, csv);
        }
        catch (Exception ex) {
            Console.WriteLine("An error occurred while creating the file: " + ex.Message);
        }
    }

    private static void SendFiles() {
        // Este método enviará los archivos generados
        // Deberías completar esta función de acuerdo a tus necesidades
    }
    
    private static string ConvertToCsv(DataTable table) {
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
