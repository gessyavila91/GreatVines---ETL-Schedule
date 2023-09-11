namespace GreatVibesSchedule.helper;

using System.Text;
using System.Data;
using System.IO;
using System.IO.Compression;

public static class GreatVibes {
    
    private static readonly string dateFormat = "yyyyMMdd_HHmmss";
    private static readonly string StoreProcedurePrefix = "SAP_GreatVines_";
    private static List<string> storedProceduresList = new() { "SAP_GreatVines_Products", "SAP_GreatVines_Distributors"};
    private static List<string> csvFilesList;
    private static DateTime now = DateTime.Now;
    
    private static string clientName = "ClaseAzul";
    
    static GreatVibes() {

    }

    public static void CreateFiles() {
        
        List<string> createdFiles = new List<string>();
        csvFilesList = null;

        foreach (string storedProcedure in storedProceduresList) {
            try {
                string fileName = GetCSVFileName(now, storedProcedure);
                string csvContent = ConvertToCsv(SqlServer_Helper.ExecuteStoredProcedure(storedProcedure));
                CreateFiles(csvContent, fileName);
                createdFiles.Add(fileName);

            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }
        csvFilesList = createdFiles;
    }

    public static void PrintFiles() {
        if (csvFilesList != null) {
            try {
                foreach (string file in csvFilesList) {
                    Console.WriteLine(file);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }
    }
    public static void SaveFilesToZip() { // Declara un método estático llamado SaveFilesToZip, que acepta un argumento opcional que especifica la ruta del archivo zip a crear.
        string zipPath = GetZipName();
        
        try { // Inicia un bloque try para manejar cualquier excepción que pueda ocurrir en el código que está dentro de este bloque.
            if (csvFilesList != null) { // Comprueba si csvFilesList no es null, para evitar una excepción NullReferenceException en el bucle foreach a continuación.
                using (ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Update)) { // Abre el archivo zip especificado por zipPath para actualizarlo. Crea uno si aún no existe.
                    foreach (string file in csvFilesList) { // Itera sobre cada string en csvFilesList, que se supone que son rutas de archivos csv.
                        if (File.Exists(file)) { // Comprueba si el archivo csv actual existe en el sistema de archivos.
                            zip.CreateEntryFromFile(file, Path.GetFileName(file)); // Si existe, añade este archivo al archivo zip, usando el nombre del archivo csv como nombre de la entrada en el archivo zip.
                        }
                    }
                }
            }
        } catch (Exception e) {
            Console.WriteLine(e.Message);
            throw;
        }
    }
    
    public static string GetCSVFileName(DateTime now, string file = null) {
        return file == null ? $"{clientName}_{now.ToString(dateFormat)}.csv" :
            $"{clientName}_{file}_{now.ToString(dateFormat)}.csv";
    }
    
    public static string GetZipName() {
        return$"{clientName}_{now.ToString(dateFormat)}.zip";
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

    private static string ConvertToCsv(DataTable table) {
        StringBuilder csv = new StringBuilder();

        getCsvHeader(table, csv);
        getCsvBody(table, csv);

        return csv.ToString();
    }
    private static void getCsvBody(DataTable table, StringBuilder csv) {
        // Escribir el contenido
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
    }
    private static void getCsvHeader(DataTable table, StringBuilder csv) {
        // Escribir los nombres de las columnas
        for (int i = 0; i < table.Columns.Count; i++) {
            csv.Append(table.Columns[i].ColumnName);
            csv.Append(i == table.Columns.Count - 1 ? "\n" : ",");
        }
    }
    
    private static void SendFiles() {
        // Este método enviará los archivos generados
        // Deberías completar esta función de acuerdo a tus necesidades
    }

}
