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
            ConsoleColor temp = Console.ForegroundColor;
            string msg = $"[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} ERROR]: {e.Message} {e.TargetSite}";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Log(msg);
            Console.ForegroundColor = temp;
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

        public static void Debug(string message)
        {
            if (Config.Debug)
            {
                ConsoleColor temp = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                message = $"[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} DEBUG] {message}";
                Console.WriteLine(message);
                Console.ForegroundColor = temp;
                Log(message);
            }
        }

        public static void Write(string message)
        {
            message = $"[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} LOG] {message}";
            Console.WriteLine(message);
            Log(message);
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
