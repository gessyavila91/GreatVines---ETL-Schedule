using System;
using System.IO;
using Renci.SshNet;

namespace SftpExample
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = "sftp.greatvines.com";
            int port = 22; // El puerto por defecto para SFTP es 22
            string username = "claseazul@greatvines.com";
            string password = "85NGAOtVxDMqkQMWJKsVPXKKe";
            string remoteDirectory = "/";
            
            string localFilePath = @"C:\archivo_a_subir.zip";

            
            using (var sftp = new SftpClient(host, port, username, password))
            {
                sftp.Connect();
                
                listFiles(sftp, remoteDirectory);
                downloadFile(sftp, remoteDirectory);
                UploadFile(sftp, localFilePath, remoteDirectory);

                sftp.Disconnect();
            }

            Console.WriteLine("¡Terminado!");
        }
        private static void listFiles(SftpClient sftp, string remoteDirectory) {

            // Listar archivos del directorio
            var files = sftp.ListDirectory(remoteDirectory);
            foreach (var file in files) {
                Console.WriteLine(file.Name);
            }
        }
        private static void downloadFile(SftpClient sftp, string remoteDirectory) {

            // Descargar un archivo
            string remoteFileName = "nombre_del_archivo.txt";
            string localPath = @"C:\ruta\local\del\archivo.txt";
            using (var fileStream = new FileStream(localPath, FileMode.Create)) {
                sftp.DownloadFile(remoteDirectory + remoteFileName, fileStream);
            }
        }
        
        private static void UploadFile(SftpClient sftp, string localFilePath, string remoteDirectory)
        {
            sftp.Connect();

            // Si el directorio remoto no existe, lo crea.
            if (!sftp.Exists(remoteDirectory))
            {
                sftp.CreateDirectory(remoteDirectory);
            }

            using (var fileStream = new FileStream(localFilePath, FileMode.Open))
            {
                sftp.UploadFile(fileStream, remoteDirectory + Path.GetFileName(localFilePath));
            }
            
        }
    }
}
