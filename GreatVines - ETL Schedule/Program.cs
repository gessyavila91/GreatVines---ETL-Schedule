namespace GreatVibesSchedule;

using System;
using System.IO;
using Renci.SshNet;
using GreatVibesSchedule.helper;

class Program {
    static void Main(string[] args) {
        SFTP_Helper SFTPHelp = new SFTP_Helper();
        SFTPHelp.sftpTEST();
        GreatVibesFlow();
    }

    private static void GreatVibesFlow() {
        GreatVibes.PrintFiles();
        GreatVibes.CreateFiles();
        GreatVibes.SaveFilesToZip();
    }

}
