using System.Reflection;
namespace GreatVibesSchedule.helper;

using System;
using System.IO;
using Renci.SshNet;


public class SFTP_Helper {
    string host = @"files.andavisolutions.com";
    int port = 22;
    string username = @"greatvinesclientdataprod.claseazul";
    string password = @"/TXX1+R1639zjpGeN3/htM3IJDz/Dlk6";
    string remoteDirectory = "/";

    string localFilePath = @"./";
    
    

    public void sftpTEST() {
        ConnectionInfo connInfo = new ConnectionInfo(host, port, username, new PasswordAuthenticationMethod(username, password));

        try {
            // Get the SFTP client instance
            using (var sftp = new SftpClient(connInfo)) {
                // Connect to the server
                sftp.Connect();

                // Check if we're connected
                if (sftp.IsConnected) {
                    Console.WriteLine("Connected to the server.");
                    sftp.ListDirectory(remoteDirectory);
                    Console.WriteLine(sftp.ListDirectory(remoteDirectory));
                }
                else {
                    Console.WriteLine("Could not connect to the server.");
                }

                // Disconnect
                sftp.Disconnect();
            }

        } catch (Exception ex) {
            ErrorHandler_Helper.HandleException(ex, MethodBase.GetCurrentMethod().DeclaringType.Name,
                MethodBase.GetCurrentMethod().Name);
        }
    }
    private void listFiles(SftpClient sftp, string remoteDirectory) {

        // Listar archivos del directorio
        var files = sftp.ListDirectory(remoteDirectory);
        foreach (var file in files) {
            Console.WriteLine(file.Name);
        }
    }
    private void downloadFile(SftpClient sftp, string remoteDirectory) {

        // Descargar un archivo
        string remoteFileName = "nombre_del_archivo.txt";
        string localPath = @"C:\ruta\local\del\archivo.txt";
        using (var fileStream = new FileStream(localPath, FileMode.Create)) {
            sftp.DownloadFile(remoteDirectory + remoteFileName, fileStream);
        }
        Console.WriteLine("¡Descarga Terminada!");
    }

    private void UploadFile(SftpClient sftp, string localFilePath, string remoteDirectory) {
        sftp.Connect();

        // Si el directorio remoto no existe, lo crea.
        if (!sftp.Exists(remoteDirectory)) {
            sftp.CreateDirectory(remoteDirectory);
        }

        using (var fileStream = new FileStream(localFilePath, FileMode.Open)) {
            sftp.UploadFile(fileStream, remoteDirectory + Path.GetFileName(localFilePath));
        }

    }
}
