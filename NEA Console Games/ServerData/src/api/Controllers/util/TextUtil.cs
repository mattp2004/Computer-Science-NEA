using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.api.Controllers.util
{
    public class TextUtil
    {
        public static ConsoleColor defaultColor = ConsoleColor.White;
        public static void WriteLine(string message, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
            Console.WriteLine();
        }
    }
}
