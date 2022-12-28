using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.misc
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
            Console.ForegroundColor = temp;
        }
        public static string SQLDate(DateTime dateTime)
        {
            return $"{dateTime.Year}-{dateTime.Month}-{dateTime.Day}";
        }
    }
}
