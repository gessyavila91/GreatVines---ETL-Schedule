using System.Reflection;
namespace GreatVibesSchedule.helper;

using System;
using System.IO;
using Renci.SshNet;


public static class SFTP_Helper {
    private static string host = @"files.andavisolutions.com";
    private static int port = 22;
    private static string username = @"greatvinesclientdataprod.claseazul";
    private static string password = @"/TXX1+R1639zjpGeN3/htM3IJDz/Dlk6";

    public static void sftpTEST() {
        try {
            using (var sftp = new SftpClient(ConnectionInfo())) {
                sftp.Connect();
                if (sftp.IsConnected) {
                    Console.WriteLine("Connected to the server.");
                    foreach (var file in sftp.ListDirectory("/")) {
                        Console.WriteLine((file.IsDirectory ? "Directory: " : "File:     ") + file.FullName);
                    }
                } else {
                    Console.WriteLine("Could not connect to the server.");
                }
                sftp.Disconnect();
            }

        } catch (Exception ex) {
            ErrorHandler_Helper.HandleException(ex, MethodBase.GetCurrentMethod().DeclaringType.Name,
                MethodBase.GetCurrentMethod().Name);
        }
    }
    public static void listFiles(SftpClient sftp, string remoteDirectory) {
        try {
            var files = sftp.ListDirectory(remoteDirectory);
            foreach (var file in files) {
                Console.WriteLine(file.Name);
            }
        } catch (Exception ex) {
            ErrorHandler_Helper.HandleException(ex, MethodBase.GetCurrentMethod().DeclaringType.Name,
                MethodBase.GetCurrentMethod().Name);
        }
    }
    public static void downloadFile(SftpClient sftp, string remoteDirectory) {
        try {
            string remoteFileName = "nombre_del_archivo.txt";
            string localPath = @"C:\ruta\local\del\archivo.txt";
            using (var fileStream = new FileStream(localPath, FileMode.Create)) {
                sftp.DownloadFile(remoteDirectory + remoteFileName, fileStream);
            }
            Console.WriteLine("¡Descarga Terminada!");
        } catch (Exception ex) {
            ErrorHandler_Helper.HandleException(ex, MethodBase.GetCurrentMethod().DeclaringType.Name,
                MethodBase.GetCurrentMethod().Name);
        }
    }

    public static void UploadFile(string localFilePath, string remoteDirectory) {
        try {
            using (var sftp = new SftpClient(ConnectionInfo())) {
                sftp.Connect();
                if (!sftp.Exists(remoteDirectory)) {
                    sftp.CreateDirectory(remoteDirectory);
                }

                using (var fileStream = new FileStream(localFilePath, FileMode.Open)) {
                    sftp.UploadFile(fileStream, remoteDirectory + Path.GetFileName(localFilePath));
                }
                sftp.Disconnect();
            }
        } catch (Exception ex) {
            ErrorHandler_Helper.HandleException(ex, MethodBase.GetCurrentMethod().DeclaringType.Name,
                MethodBase.GetCurrentMethod().Name);
        }

    }

    private static ConnectionInfo ConnectionInfo() {
        return new ConnectionInfo(host, port, username, new PasswordAuthenticationMethod(username, password));
    }
    
}
