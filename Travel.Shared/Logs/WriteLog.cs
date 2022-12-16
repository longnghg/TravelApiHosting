using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.Logs
{
    public static class WriteLog
    {
        //public static void WriteLog(string message, string folder = "Logs")
        //{
        //    string path = $"{AppDomain.CurrentDomain.BaseDirectory}//{folder}//";
        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }
        //    string file = $"{path}Logs.txt";
        //    if (!File.Exists(file))
        //    {
        //        using (StreamWriter sw = File.CreateText(file))
        //        {
        //            sw.WriteLine(message);
        //        }
        //    }
        //    else
        //    {
        //        using (StreamWriter sw = File.AppendText(file))
        //        {
        //            sw.WriteLine(message);
        //        }
        //    }
        //}
    }
}
