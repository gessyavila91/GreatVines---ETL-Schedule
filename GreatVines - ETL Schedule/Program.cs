namespace GreatVibesSchedule;

using System;
using System.IO;
using Renci.SshNet;
using GreatVibesSchedule.helper;

class Program {
    static void Main(string[] args) {
        GreatVibesFlow();
    }

    private static void GreatVibesFlow() {
        GreatVibes.PrintFiles();
        GreatVibes.CreateFiles();
        GreatVibes.SaveFilesToZip();
        GreatVibes.SendFiles();
    }

}
