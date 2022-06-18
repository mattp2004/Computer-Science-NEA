using GameServer.src.config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.src.misc
{
    class Util
    {
        public static string CurrentLogName = "null";
        public static void Error(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR OCCURED: {e.Message} {e.TargetSite}");
            Log(e.Message);
        }

        public static void Log(string message)
        {
            if (!File.Exists(CurrentLogName))
            {
                GenerateLog();
            }
            using (StreamWriter sw = new StreamWriter(CurrentLogName, true))
            {
                sw.WriteLine(message);
            }
        }

        public static void GenerateLog()
        {
            if (CurrentLogName == "null")
            {
                CurrentLogName = $"{Config.logsName}/Log {DateTime.Now.Date.Day}-{DateTime.Now.Date.Month}-{DateTime.Now.Date.Year} {DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}.txt";
                using (StreamWriter sw = new StreamWriter(CurrentLogName)) { }
            }
        }

        public static void GenerateLogFolder()
        {
            System.IO.Directory.CreateDirectory(Config.logsName);
        }
    }
}
