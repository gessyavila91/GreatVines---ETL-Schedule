﻿using System.Reflection;
namespace GreatVibesSchedule.helper;

using System.Text;
using System.Data;
using System.IO;
using System.IO.Compression;

public static class GreatVibes {
    
    private static readonly string dateFormat = "yyyyMMdd_HHmmss";
    private static readonly string StoreProcedurePrefix = "SAP_GreatVines_";
    private static List<string> storedProceduresList = new()
    { "SAP_GreatVines_Contact"
     ,"SAP_GreatVines_Distributors"
     ,"SAP_GreatVines_Inventory"
     ,"SAP_GreatVines_Orders"
     ,"SAP_GreatVines_Products"
     ,"SAP_GreatVines_Sales_Orders"
     ,"SAP_GreatVines_Sales_Orders_Items"
    };
    private static List<string> csvFilesList;
    private static DateTime now = DateTime.Now;
    
    private static string clientName = "ClaseAzul";

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
        if (getcsvWIPFiles() != null) {
            try {
                foreach (string file in getcsvWIPFiles()) {
                    Console.WriteLine(file);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    private static List<string> getcsvWIPFiles() {
        return csvFilesList;
    }
    public static void SaveFilesToZip() {
        string zipPath = GetZipName();

        if (csvFilesList == null) return;

        try {
            using (ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Update)) {
                foreach (string file in csvFilesList) {
                    if (!File.Exists(file)) continue;

                    zip.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }
        }
        catch (Exception e) {
            Console.WriteLine(e.Message);
            throw;
        }
    }
    
    private static string GetCSVFileName(DateTime now, string file = null) {
        return file == null ? $"{clientName}_{now.ToString(dateFormat)}.csv" :
            $"{clientName}_{file}_{now.ToString(dateFormat)}.csv";
    }
    
    private static string GetZipName() {
        return$"{clientName}_{now.ToString(dateFormat)}.zip";
    }
    
    private static void CreateFiles(string csv, string fileName, string filePath = ".") {
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
    
    public static void SendFiles() {
        // Check if the file exists
        if (File.Exists(GetZipName())) {
            Console.WriteLine("File exists.");
            SFTP_Helper.UploadFile(GetZipName(),"/claseazul@greatvines.com/FTP/");

            RemoveFiles();

        } else {
            Console.WriteLine("File does not exist.");
        }
    }

    private static void RemoveFiles() {
        try {
            string localDirectory = "./";

            string[] zipFiles = Directory.GetFiles(localDirectory, "*.zip");
            string[] csvFiles = Directory.GetFiles(localDirectory, "*.csv");

            var files = zipFiles.Concat(csvFiles);

            foreach (var file in files) {
                if (File.Exists(file)) {
                    File.Delete(file);
                    Console.WriteLine("Deleted: " + file);
                }
            }
        } catch (Exception ex) {
            ErrorHandler_Helper.HandleException(ex, MethodBase.GetCurrentMethod().DeclaringType.Name,
                MethodBase.GetCurrentMethod().Name);
        }
    }
    
}
