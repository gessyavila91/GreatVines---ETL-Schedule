namespace GreatVibesSchedule;

using System;
using System.IO;
using Renci.SshNet;
using GreatVibesSchedule.helper;

class Program {
    static void Main(string[] args) {

        string host = "192.168.1.21";
        string database = "SBO_Tequilaspremium_Prod";
        string userId = "Desarrollos";
        string password = "D3v5ll0@7r@22$";

        GreatVibes.CreateFiles();
    }

}
