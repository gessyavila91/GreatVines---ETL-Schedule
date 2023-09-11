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
            
            using (var sftp = new SftpClient(host, port, username, password))
            {
                sftp.Connect();
                
                // Listar archivos del directorio
                var files = sftp.ListDirectory(remoteDirectory);
                foreach (var file in files)
                {
                    Console.WriteLine(file.Name);
                }
                
                downloadFile(sftp, remoteDirectory);

                sftp.Disconnect();
            }

            Console.WriteLine("¡Terminado!");
        }
        private static void downloadFile(SftpClient sftp, string remoteDirectory) {

            // Descargar un archivo
            string remoteFileName = "nombre_del_archivo.txt";
            string localPath = @"C:\ruta\local\del\archivo.txt";
            using (var fileStream = new FileStream(localPath, FileMode.Create)) {
                sftp.DownloadFile(remoteDirectory + remoteFileName, fileStream);
            }
        }
        
        private static void UploadFile(string host, int port, string username, string password, string localFilePath, string remoteDirectory)
        {
            using (var sftp = new SftpClient(host, port, username, password))
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

                sftp.Disconnect();
            }
        }
    }
}
